extends Resource
class_name TextureManifest

const WALLPAPER := "res://assets/textures/wallpaper_yellow_color.png"
const CARPET_PLACEHOLDER := "res://assets/textures/carpet_dirty_color.png"
const CEILING_PLACEHOLDER := "res://assets/textures/ceiling_tiles_color.png"
const GRIME_PLACEHOLDER := "res://assets/textures/grime_stain_color.png"
const PLASTIC_PLACEHOLDER := "res://assets/textures/plastic_panel_color.png"

const UPLOADED_DIR := "res://assets/textures/uploaded/"
const UPLOADED_CARPET_COLOR := UPLOADED_DIR + "carpet_fabric_color.jpg"
const UPLOADED_CARPET_NORMAL := UPLOADED_DIR + "carpet_fabric_normal_gl.jpg"
const UPLOADED_WALL_LEAK_COLOR := UPLOADED_DIR + "wall_leak_color.jpg"
const UPLOADED_WALL_LEAK_OPACITY := UPLOADED_DIR + "wall_leak_opacity.jpg"
const UPLOADED_CEILING_COLOR := UPLOADED_DIR + "office_ceiling_color.jpg"
const UPLOADED_METAL_COLOR := UPLOADED_DIR + "painted_metal_color.jpg"
const UPLOADED_PLASTIC_COLOR := UPLOADED_DIR + "plastic_panel_color.jpg"
const UPLOADED_WOOD_COLOR := UPLOADED_DIR + "wood_trim_color.jpg"

static func existing_texture(path: String) -> Texture2D:
    if ResourceLoader.exists(path):
        return load(path)
    return null
