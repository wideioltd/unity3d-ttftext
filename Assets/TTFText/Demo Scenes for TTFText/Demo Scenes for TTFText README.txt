TTFText for Unity 2.0
#################################################################
Created by ComputerDreams.org
Maintainer : B.Nouvel
Authors : B.Nouvel, O.Blanc
################################################################
Latest infos available from http://unityttf.computerdreams.org/
################################################################

This folder contains three simple demos for Unity TTF Text Package.

To visualize these demos correctly we recommend to switch your "Player Settings" to "Deferred Lighning".

Moreover, because we are dealing with text : Adding an Antialias filter on the camera is strongly recommended.
The demo scene never includes it as it is a Pro Only asset.


Scene 1   : It is simply an example of static mesh. It has been generated from the editor. It does not move.
      1_j : Same thing with an asiatic text.
Scene 2 : It is a dynamical scene, it is a particle generator sending letters (but you can change it to be words you like), 
		  using all the fonts available on your system, and demonstrating the capacity of TTFText by 
		  applying different random extrusions to them.
Scene 3 : It is an example of very simple text animation, in this case the mesh is actually statically generated,
          but one subobject have been generated for each characted based on a prefab, and by the mean of this 
		  prefab we animat independently  each character (simply scaling it along its extrusion here).
DemoClothBased : This simply shows two things :
                       - You may use different component to interact with mesh (not only a mesh filter and a mesh renderer)
                       - By associating a prefab to each letter, and by associating a script to that prefab,  you can obtain a sequential behaviour on the text.
LongText : 
          This is an attempt to render a quite long text with TTF Text.
          The meshes associated with the text are dynamically generated when you click on the button
StyleAndMarkup :
          These are two test /demo scenes that shows example of Styles, and the way they may be used.
TextEffects :
          This show some preprogrammed effects based on prefab scripts
Web 1: 
          This demo demonstrate how to create an interactive demo for a web application.
          Note that in current version - you must ensure that the font are embedded in your project.
          Go to ADVANCED : Select Show System Objects - Go to TTF Text Font Store. Click on "Embed All Project Fonts".
Web 2:          
          This another demo that demonstrate how to create an interactive demo for a web application.
          Note that in current version - you must ensure that the font are embedded in your project.
          Go to ADVANCED : Select Show System Objects - Go to TTF Text Font Store. Click on "Embed All Project Fonts".
