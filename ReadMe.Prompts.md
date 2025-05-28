## Source of truth

https://github.com/willwulfken/MidJourney-Styles-and-Keywords-Reference

## Describe Prompts 

```
/imagine prompt: wetlands in the woods, hooded slim person on a fallen tree, Dark Fantasy, Painting By Albert Bierstadt --quality 1 --stylize 2500 --aspect 16:9
```

1. the -- variables needs to be set at the end of the prompt
2. --quality 1     		
    is the default one. We can use quality 0.5 or 2. Bigger quality, more time used to generate image, but the image is better
3. --stylize 2500    
    is the default one. We can use stylize 1250 or 20000. Bigger stylize, more chaotic image is and less connected to the keywords
4. --aspect 16:9         
    is an aspect ratio, to the width and height of the image

```
/imagine hot dog::1.5 food::-1
```

we can set weights to the words, to make them more important than other.  They are a percentage of a sum of all weights.

If the weight is not specified, it defaults to 1

## Additional info

To obtain a seed from a upscaled image -> right click on it -> add reaction -> search "env" -> select koperta (zwyk³a)

to use the seed: "--seed <seed number>" (it will result in 4 images that are inspired on a image)

use "--sameseed <seed number>" (it will result in 4 almost same images based on a seed!!)

We can combine multiple prompts:
```
[goddes of the lost, full body, silk, gold] + [lost city background, soft bokeh, eerie light] --aspect 9:16 --quality 1 --stylize 2500
```
(so properties are one, commands can be two

We can also use weights:
```
[goddes of the lost, full body, silk, gold]::5 + [lost city background, soft bokeh, eerie light]::2 --aspect 9:16 --quality 1 --stylize 2500
```