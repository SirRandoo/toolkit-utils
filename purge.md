---
layout: default
title: Purge Viewers
nav_order: 6
---

## Purge Viewers

![Purge Button in Viewers Window]({{- "/assets/viewers/purge.png" | absolute_url -}})

The above image depicts Toolkit's viewer editor window. When ToolkitUtils
is installed, you'll notice a "purge" button in the top-right corner of
the window. Clicking this button allows you to open Utils' purge window.

![Purge Window]({{- "/assets/viewers/purge_constraints.png" | relative_url -}})

The above image depicts the window shown after the purge button has been
clicked. This window allows you to set constraints for your purge.

| Comparison Type | Description                                                                                             |
|:----------------|:--------------------------------------------------------------------------------------------------------|
| >=              | The value the constraint compares against must be greater than or equal to the value of the constraint. |
| >               | The value the constraint compares against must be greater than the value of the constraint.             |
| =               | The value the constraint compares against must be the value of the constraint.                          |
| <=              | The value the constraint compares against must be less than or equal to the value of the constraint.    |
| <               | The value the constraint compares against must be less than the value of the constraint.                |
| is              | This comparison is similar to `=` in that the values must match.                                        |
| is not          | This comparison is the inverse of `is` in that the values must **not** match.                           |
| contains        | This comparison checks to see if the constraint's value is _in_ the value.                              |
| startswith      | This comparison checks to see if the value starts with the constraint's value.                          |
| endswith        | This comparison checks to see if the value ends with the constraint's value.                            |

![Purge Window Review]({{- "/assets/viewers/purge_review.png" | relative_url -}})

The above image depicts the window shown after the "purge viewers" button
has been clicked. This screen allows you to excempt affected viewers from
being purged. Clicking the "confirm" purges all viewers left on the screen.
