--------------------------
Boxed Convex Collider
--------------------------

--------------------------
How to use
--------------------------

Two ways to use:

#1.

1. Select an object and add the component Boxed Convex Collider.
2. Set the properties.
3. Press Calculate Colliders.

#2.

1. Go to Window/Boxed Convex Collider and open the window.
2. Select the object the calculate. All layers of children including itself will be checked,
   if the object has the Mesh Filter component it will be calculated.
3. Set the properties.
4. Press Calculate Colliders.

--------------------------
How it works
--------------------------

1. Calculates the bounds of the mesh.
2. Generates a box collider at different positions of the mesh to check if it collides with the mesh.
3. Gets the collided positions and check for adjacent collided positions to combined into rectangles
   to generate into meshes.

--------------------------
Properties
--------------------------

Collider Precision: The scale of the boxes used to calculate. The smaller the value the more accurate the
   the gerenated mesh will be, although more components will be generated causing decreased performance.
   Value range is 0.0001 to 65536.
Merge Precision: The maximum amount of time the script will try to merge for a larger rectangle. 
   Recommend value is 0.01 for faster calculations. Value range if 0.01 to 100.
Collider Min Size: The minimum amount of boxes needed in a rectangle for the mesh to be generated.
   Increasing the value will result in lower total mesh count therefore better preformance.
   This value can be increased later after calculating the mesh (but not decreased), so recommended starting
   with a low value. Value range is 1 to 65536.

--------------------------
Other notes
--------------------------

1. If mesh counts are to large, it may cause in a performance decrease and with large scenes files ect.
   Mesh counts can be lowered with lower collider precision or by increasing the collider minimum size.
   Some tested results have lowered performance starting at a mesh count at 500, although may vary with
   different devices.