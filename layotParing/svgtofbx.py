'''
# How to use
run below command at the parent directory of svg files!
`blender -b -P svg2fbx.py`
if blender exec path is not defined, please run below one. (tested only on MacOS)
`/Applications/Blender/blender.app/Contents/MacOS/blender -b -P svg2fbx.py`
'''

import os
import bpy
import glob

SVG_DIRECTORY_PATH = './'
EXPORT_FBX_PATH = 'export/'

# change current working directory to python/
cwd = os.getcwd()
os.chdir(cwd)

# check whether directories are exist
if not os.path.isdir(EXPORT_FBX_PATH):
    os.mkdir(EXPORT_FBX_PATH)


def delete_all_objects():
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete()


def delete_objects(obj_type):
    # select objects by type
    for obj in bpy.context.scene.objects:
        if obj.type == obj_type:
            obj.select = True
        else:
            obj.select = False

    # delete selected objects
    bpy.ops.object.delete()


def set_to_origin():
    # set object position to origin
    bpy.ops.object.origin_set(type='ORIGIN_GEOMETRY')

    for window in bpy.context.window_manager.windows:
        screen = window.screen
        for area in screen.areas:
            if area.type == 'VIEW_3D':
                for region in area.regions:
                    if region.type == 'HEADER':
                        override = {'window': window, 'screen': screen, 'area': area, 'region': region}
                        bpy.ops.view3d.snap_selected_to_cursor(override, use_offset=True)
                    break


def set_scale(obj, scale):
    obj.scale *= scale


def set_color(obj, r, g, b):
    obj.data.materials[0].diffuse_color = (r, g, b)


def curve2mesh():
    # omajinai for converting mesh...
    svg_obj = bpy.data.objects['Curve']
    svg_obj.select = True
    bpy.context.scene.objects.active = bpy.context.selected_objects[0]

    # convert to mesh
    bpy.ops.object.convert(target='MESH')

    # separate mesh
    bpy.ops.mesh.separate(type='LOOSE')


def make_uv_maps(obj):
    bpy.context.scene.objects.active = obj
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.uv.smart_project()
    bpy.ops.object.mode_set(mode='OBJECT')


def export_fbx_from_svg(svg_file_path, export_fbx_file_path):
    # load svg
    bpy.ops.import_curve.svg(filepath=svg_file_path)

    bpy.ops.object.select_all(action='SELECT')
    for obj in bpy.context.selected_objects:
        # change diffuse color
        set_color(obj, 1.0, 1.0, 1.0)

        # change scale
        set_scale(obj, 10)

    # set object position to origin
    set_to_origin()

    # convert curve to mesh
    curve2mesh()

    # delete noise curve objects
    delete_objects('CURVE')

    #  make smart uv
    bpy.ops.object.select_all(action='SELECT')
    selected_objects = bpy.context.visible_objects
    for obj in selected_objects:
        make_uv_maps(obj)

    # export fbx
    bpy.ops.export_scene.fbx(filepath=export_fbx_file_path)


# ======================================================
# process!!!!!!

# delete objects in scene for initializing
delete_all_objects()

# load svg files
svg_file_paths = glob.glob(SVG_DIRECTORY_PATH + '*.svg')

# export fbx files from svg
for svg_file_path in svg_file_paths:
    name, ext = os.path.splitext(os.path.basename(svg_file_path))
    export_fbx_file_path = EXPORT_FBX_PATH + name + '.fbx'
    export_fbx_from_svg(svg_file_path, export_fbx_file_path)

    # delete already exported fbx object in blender
    delete_all_objects()
