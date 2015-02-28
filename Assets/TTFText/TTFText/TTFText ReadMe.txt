TTFText for Unity 2.0 pre 1
#################################################################
Maintained by WIDE-IO CONSULTING LTD


Maintainers : B.Nouvel
Original authors : B.Nouvel, O.Blanc
################################################################
Latest infos available from http://unityttf.computerdreams.org/
################################################################

TTFText package allows to easily build meshes rendering a text in 3D with your selected
truetype font directly in the Unity Editor. It also include and experimental support for 
bitmap fonts that may be used to access native fonts on Android and IPhone.

TTFText also supports some other vectorial formats such as OpenType and Adobe Type 1, 
and those that are supported by specific mobile devices.

Usage
=====

To create a new TTF Text select the menu entry "GameObject/Create Others/TTF Text".

A new gameobject will be created with the a TTFText Component.
The mesh will be updated and displayed in the scene view as you modify its 
your parameters.


Text
----

Specify the text to render.


Embed font
----------
When this is enabled the system will also copy the font towards other platform. 
It means the fonts will be embedded, and will be usable on mobile platforms.
Note that by default only the ASCII characters are included. If you want to change 
this for the moment you need to enable the engine specific options and to specify the list 
of included fonts.

Platform Specific
-----------------
When this option is enabled you are allowed to select a different font on each "engine".
A font "engine" is the set of algorithm that are used to access fonts. Different font engine have different
features.
You can select which engine is associated to which platform by going in the advanced option pan.



Font Selection
--------------

Select the font you which to use from all available TTF fonts present in your System  (or restrict only to those in the asset DataBase).
You can toggle from one view to the other by pressing the button "Show System Font"/"Hide System Fonts".
If you want to use a font that is located elsewhere, just add it as an asset by clicking the  "Add Font" button.



Font Size
---------

Specify the font size (in unity units), note that this specify a "nominal size" specific to the font
and does not correspond to the size of any character.
If you want to have better control on the exact dimensions of the generated mesh, you can
tune this in the "Mesh Boundaries" section.




One GameObject Per
------------------
The default value for this option is "Text" meaning that the whole input text is generated as a single mesh.
But in many cases, it is useful for algorithmic, gamelogic, or mesh size limitations to use more than one gameobject.
This option allow you to split your text in subojects. On subobject will be created for each "character", "word", "line"
depending on your option:

In the case where you create subojects, a prefab should be passed in order to make the text visible. 
This prefab should generally contain a component to receive the mesh (Cloth or Mesh Filter), and a component
renderer (MeshRenderer or ClothRenderer).

A component TTFSubmesh will be associated to each generated gameobject. This component will allow the other scripts 
to know the text that is associated with current mesh, and its index in the generation of the different subojects.
(More info will be provided in future versions)

Have a look at the demo scene 3 for an example of text using that feature.


Prefab
------

The possibility to use a template prefab for each letter, word or line, is one of 
the key feature that allow TTF Text to be the most powerful text extension for unity.

This is the prefab used in conjunction with the previous option. See previous menu for detail.
See TextEffects demo.







Outline options
===============


Simplify Outline
----------------
By changing this parameter you can decimate some points from the outline of your text in order to reduce the complexity of 
the generated mesh.


Embold
------
Embold is an algorithm that allows you to add or remove some boldness to your fonts. It does not necessarily work perfectly
on all fonts but work nicely in many cases.

Slant
-----
As Embold allows you to change the fickness of your font, slant allows you change freely the inclination of the characters of your fonts.






Extrusion
=========

This panel allows you to define the way the Mesh is generated from the text outline.

Extrusion Mode
--------------

For convenience different modes qre provided

* None : No extrusion is performed, the text remains flat. This is the fastest.
   The parameter "backFace" allows to select whether the text is double-sided (visible from both side or not)
   If a backface is generated the backface may actually be mapped to a different texture (see Texture Mapping below).


* Simple : This allows to create a simple extrusion of the text.
  Extrusion Depth : Length of the extrusion (distance in-between front-face and back-face)

* Bevel : Creates rounded angles
  Extrusion Depth : Length of the extrusion (distance in-between front-face and back-face)
  Bevel Force     : Change in emboldment
  Steps           : Steps of interpolation in the Bevel 
  Bevel Depth %   : Percentage of the extrusion depth on which the Bevel is computed 

* Bended : Variant of bevel that is softer and spreaded along the mesh
  Extrusion Depth : Length of the extrusion (distance in-between front-face and back-face)
  Bevel Force     : Change in emboldment
  Steps           : Steps of interpolation in the Bevel 
  Gamma           : Exponential factor defining how the interpolation gets thinner as it arrives close to the angles.


* Free Hand :
  Extrusion Depth : Length of the extrusion (distance in-between front-face and back-face)
  Bevel Force     : Change in emboldment
  Curve           : Where you define the shape of the extrusion

* Pipe
  Radius          : Radius of the pipe
  Num of Edges    : Interpolation of the circles forming the border of the pipe 




Text Layout
===========



Layout
------

We provide three one-line layout mode and one paragraph layout mode (Wrap).

* Simple
    
   Char Spacing:
   -------------
   This option  specify a factor to tune the distance between characters. 


* Traditional
  Paragraph Alignment
    The following usual modes are supported : 
        Left, Center, Right, Justified, Fully Justified

* Markup Language and Styles

TTF Text markup with HTML based syntax : 

Example :
<@style=big@> <@ style = red @>  This  <@ pop @> is still big <@ pop @> !

Most properties of TTF Text are also directly accessible :
<@Size=3@>Size <@Embold=+-0.3@>3<@pop@><@pop@>

The operator =+ (resp=*) is the equivalent of +=  (resp *=) in C#.

From the version 2.0. TTF Text supports images inside of the text.
Currently the image must be mapped through a material that is passed as one of the 
material of the text. We know this situation is not ideal.
<@img @>


Texture Mapping
===============

[] Split sides
---------------

If you select this option, the generated mesh will be made of 3 submesheses, one for each side of the text (front, back, side?).
This is useful if you want to have different textures for each side of the mesh.

For each side, you can also adapt the uv mapping coordinate by your specified factor.



UV Mapping
----------

* Box
  In box mode the UV are changing linearly with x and y coordinates in the mesh. Z is affecting both u and v at the same time lineraly too.
  u and v are computed either directly from the vertice coordinate in a real world space, either from a space normalized at the size of mesh.

* Spherical
  In Spherical the UV maps are computed as projected from a sphere :
  according to the euler angles linking an azimuth direction the vector form the center of the 3D mesh, and the vertice considered in that mesh>.

* uvScaling
  In both of the previous mode the value computed can be scaled using that vector.
  

3D Object Alignment
===================

Horizontal Position, Vertical Position:
-------------------------------------------
These parameters set the position of the builded mesh relative to the transform of the GameObject.
By default, the transform corresponds the center of the mesh.

Advanced
========
Prefered Triangulation
----------------------

On PC & Mac, you may select if you prefer to use a native DLL (GLUT), or
if you prefer to use an algorithm written in C#.

Easy deployement (Work In Progress)
----------------
If this option is enabled TTF Text will try to include the font you are using in the 
distributed executable.

Coroutine based rendering
-------------------------
Computing the triangulation for all the letters may represent a lot of work.
When this option is enabled, TTFText splits the work of creating the text along different frame.
This may help you to maintain a good framerate on platforms with slow CPU.

Automatic rebuilds
------------------
Disable this if the rendering is too slow and you want to change lots of parameters on your object in the editor


Style Prefab Edit Mode
----------------------
In edit mode, if you select a style prefab, and this option is enable the text using this style will be constantly rebuilt.
If for some reason you need to disable this behaviour, you may untick this box.


Show system objects
-------------------
Allows you to see the style collection - as well as the TTF Text Font Store.

Engine Selection
----------------
Allows you to define which font engine is to be used for each platform.
 
 
Finally
========
 
 Save Mesh Button:
 -----------------
 By clicking this button, you can save the generated mesh as a new asset so that it can easily be reused by other
 components in your project.
 
 Add Font
 -----------
 Use this button to import a True Type font as an asset in your project.
 
 
#######################################################################
Acknowledgment:
---------------
Special thanks to "EG" for creating TTFText Logo and first images.
Very great thanks also to the developpers of the free libraries
that form one of the essential basis of this software : Freetype, Clipper and
Poly2tri.
A huge thanks to Mario Freitas for Sysfont - which is partially included as part of TTFText.

Suggestions, bugs, feedbacks:
-----------------------------
If you have any inquiry the authors may be contacted at ttftext@computerdreams.org 
