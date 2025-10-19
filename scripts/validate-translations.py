# Validates translations within the solution by ensuring translation keys
# correspond to a translation contained within the mod's translation files.
import sys
from argparse import ArgumentParser
from dataclasses import dataclass
from pathlib import Path
from typing import Final
from xml.etree import ElementTree

EXCLUDED_DIRECTORY_NAMES = frozenset({"bin", "obj", "Libs", "Debug", "Release"})


@dataclass(
    frozen=True,
    slots=True,
)
class TranslationIndex:
    file: Path
    line: int
    start: int
    end: int
    key: str


def gather_translations(root: Path) -> dict[str, str]:
    container: dict[str, str] = {}

    for directory, directories, files in root.walk():
        for file in files:
            current_file = directory.joinpath(file)

            if current_file.suffix != ".xml":
                continue

            with current_file.open("r", encoding="utf-8") as in_file:
                tree = ElementTree.parse(in_file)
                root_node = tree.getroot()

                container.update({node.tag: node.text for node in root_node})

    return container


def sniff_translation_key(line: str) -> list[tuple[int, int, str]]:
    marker_key: Final[str] = "service.GetTranslation(".casefold()

    buffer: str = ""
    start_index: int = 0
    container: list[tuple[int, int, str]] = []

    for index, character in enumerate(line):
        stripped_line = line.lstrip()
        if stripped_line.startswith("//") or stripped_line.startswith("#"):
            continue

        if marker_key.startswith(buffer + character.casefold()):
            buffer = buffer + character.casefold()
        else:
            buffer = ""

        if buffer.casefold() == marker_key.casefold():
            start_index = index - len(buffer)

        if start_index and character == ")":
            buffer = ""

            key: str = line[start_index + 1 : index + 1]
            key = key[len(marker_key) + 1 : -2]

            if not key.casefold().startswith("TKUtils".casefold()):
                start_index = 0
                continue

            container.append((start_index, index, key))
            start_index = 0

    return container


def sniff_translation_service(lines: list[str]) -> str:
    for line in lines:
        service_index: int = line.index("ITranslationService")

        if service_index != -1:
            segment: str = line[
                service_index + len("ITranslationService") + 1 :
            ].strip()

            return segment[: segment.index(",")].strip()

    return ""


def sniff_translation_keys(root: Path) -> list[TranslationIndex]:
    container: list[TranslationIndex] = []

    for directory, directories, files in root.walk():
        if directory.name in {"bin", "obj", "Libs", "Debug", "Release"}:
            continue

        for file in files:
            current_file = directory.joinpath(file)

            if current_file.name == "Translation.cs":
                continue
            if current_file.suffix != ".cs":
                continue

            with current_file.open("r", encoding="utf-8") as in_file:
                lines: list[str] = in_file.readlines()
                service_key: Final[str] = sniff_translation_service(lines)

                for line_no, line in enumerate(in_file.readlines(), 1):
                    hits: list[tuple[int, int, str]] = sniff_translation_key(line)

                    if not hits:
                        continue

                    for hit in hits:
                        container.append(
                            TranslationIndex(
                                current_file, line_no, hit[0], hit[1], hit[2]
                            )
                        )

    return container


parser = ArgumentParser(
    "validate-translations",
    description="Validates translations within the solution by ensuring keys used exist in translation files.",
)
parser.add_argument("--translation-dir", type=Path, required=True, nargs=1)
parser.add_argument("--source-dir", type=Path, required=True, nargs=1)

result = parser.parse_args()

translations: dict[str, str] = {}

for translation_dir in result.translation_dir:
    translations.update(gather_translations(translation_dir))

translation_keys: list[TranslationIndex] = []

for source_dir in result.source_dir:
    translation_keys.extend(sniff_translation_keys(source_dir))


grouped_keys: dict[Path, list[TranslationIndex]] = {}
for translation_key in translation_keys:
    if translation_key.key not in translations:
        grouped_keys.setdefault(translation_key.file, []).append(translation_key)

if grouped_keys:
    print("Detected missing translation(s):")

for path, grouped_translation_keys in grouped_keys.items():
    print("  ", path)

    for translation_key in grouped_translation_keys:
        print(
            f"    - L{translation_key.line}:{translation_key.start}~{translation_key.end}",
            translation_key.key,
        )


sys.exit(int(bool(grouped_keys)))
