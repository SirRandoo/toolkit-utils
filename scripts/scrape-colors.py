"""
This script fetches a list of Crayola crayon colors from Wikipedia,
extracts their names and associated color values, and writes the
data into an XML file named 'Colors.xml'. The script relies on the
'requests' library for HTTP requests, 'BeautifulSoup' for HTML
parsing, and 'xml.etree.ElementTree' for generating the XML structure.
"""

from bs4 import BeautifulSoup, element
from requests import get as http_get
from sys import exit
from typing import Final
from xml.etree import cElementTree as Tree

COLOR_WIKI: Final[str] = "https://en.wikipedia.org/wiki/List_of_Crayola_crayon_colors"
response = http_get(COLOR_WIKI, timeout=30)

if not response.ok:
    print("Connection timed out!")
    print(response.status_code, response.reason)

    exit(1)

library_node = Tree.Element("ColorLibrary")
soup = BeautifulSoup(response.content, "html.parser")

table_node = soup.find("table")
table_body = table_node.find("tbody")

for node in table_body.find_all("tr", recursive=False):
    if isinstance(node, element.NavigableString):
        continue

    node: element.Tag
    descendants: list[element.Tag] = node.find_all("td", recursive=False)

    if len(descendants) < 3:
        continue

    name_node = descendants[1]
    color_node = descendants[2]

    color: str = color_node.get_text().strip()

    if "[" in color:
        color = color[: color.index("[")]

    Tree.SubElement(
        library_node,
        "Color",
        attrib={"Name": name_node.get_text().strip(), "Value": color},
    )

tree = Tree.ElementTree(library_node)
Tree.indent(tree, space=" " * 4)
tree.write("Colors.xml", encoding="utf-8")
