# PalTool

Command line program for performing certain operations on JASC and WW C&C palette files. Does not supplant basic palette editor functionality, at least not as of current.

Download for latest build (automatically generated from latest commit in `master` branch) can be found [here](https://github.com/Starkku/PalTool/releases/tag/latest).

```
Usage: PalTool.exe palette_file [output_palette_file] [-a, -m, -r, -c, -t, -s, -p]"
-m | Modify colors based on alpha.
-a [number] | Calculate alpha using average of color values. Number is what the cumulative color value is divided by to get the average, defaults to 3.
-t [hue|saturation|light|red|green|blue|alpha|brightness|rgb|gradients] threshold-value [new-alpha-value] | Modify alpha based on a threshold. Value listed after determines threshold type. Defaults to 'saturation' if invalid value is specified. Colors that pass the threshold value have their alpha set to the new value, rest are left untouched. No threshold is not valid, new alpha value defaults to 1. Both threshold & new alpha value should be numbers from 0 to 255.
-s [hue|saturation|light|red|green|blue|alpha|brightness|rgb|gradients] | Sort colors. Value listed after determines the sort method. Defaults to 'rgb' if invalid value is specified.
-g [startindex] [count] [saturation_threshold] | Sort parameters. Start color index, number of gradients and saturation split threshold for gradients sort (0-1.0), defaults to 1, 8 and 0.33 respectively.
-b | Output 6-bit RGB color components instead of 8-bit (only if JASC palette).
-d | Print debug info about palette to console.
-r | Do not save alpha values in output palette (only if JASC palette).
-c | Save output as WW palette, not JASC palette.
-p | Save 256x1px PNG image containing one pixel of each of the palette colors.
```

## Acknowledgements

This program uses code from the following open-source projects to make its functionality possible.

* Starkku.Utilities: https://github.com/Starkku/Starkku.Utilities

## License

This program is licensed under GPL Version 3 or any later version.

See [LICENSE.txt](LICENSE.txt) for more information.
