extends Panel

const CONFIG_PATH := "user://desolation_settings.cfg"

@onready var sensitivity_slider: HSlider = $VBox/SensitivitySlider
@onready var brightness_slider: HSlider = $VBox/BrightnessSlider
@onready var volume_slider: HSlider = $VBox/VolumeSlider
@onready var frame_slider: HSlider = $VBox/FrameSlider
@onready var graphics_slider: HSlider = $VBox/GraphicsScaleSlider
@onready var button_slider: HSlider = $VBox/ButtonScaleSlider
@onready var handedness_slider: HSlider = $VBox/HandednessSlider
@onready var look_zone_slider: HSlider = $VBox/LookZoneSlider
@onready var close_button: Button = $VBox/CloseButton

func _ready() -> void:
	_load_settings()
	close_button.pressed.connect(_close)
	sensitivity_slider.value_changed.connect(_save.bind("settings", "sensitivity"))
	brightness_slider.value_changed.connect(_save.bind("settings", "brightness"))
	volume_slider.value_changed.connect(_volume_changed)
	frame_slider.value_changed.connect(_frame_changed)
	graphics_slider.value_changed.connect(_graphics_changed)
	button_slider.value_changed.connect(_save.bind("controls", "button_scale"))
	handedness_slider.value_changed.connect(_save.bind("controls", "handedness"))
	look_zone_slider.value_changed.connect(_save.bind("controls", "look_zone"))

func _save(value: float, section: String, key: String) -> void:
	var cfg := ConfigFile.new()
	cfg.load(CONFIG_PATH)
	cfg.set_value(section, key, value)
	cfg.save(CONFIG_PATH)

func _volume_changed(value: float) -> void:
	_save(value, "settings", "volume")
	AudioServer.set_bus_volume_db(0, linear_to_db(max(value, 0.001)))

func _frame_changed(value: float) -> void:
	_save(value, "settings", "frame_cap")
	Engine.max_fps = int(value)

func _graphics_changed(value: float) -> void:
	_save(value, "settings", "graphics_scale")
	get_viewport().scaling_3d_scale = clamp(value, 0.6, 1.0)

func _load_settings() -> void:
	var cfg := ConfigFile.new()
	cfg.load(CONFIG_PATH)
	sensitivity_slider.value = float(cfg.get_value("settings", "sensitivity", 1.0))
	brightness_slider.value = float(cfg.get_value("settings", "brightness", 1.0))
	volume_slider.value = float(cfg.get_value("settings", "volume", 0.8))
	frame_slider.value = float(cfg.get_value("settings", "frame_cap", 60.0))
	graphics_slider.value = float(cfg.get_value("settings", "graphics_scale", 1.0))
	button_slider.value = float(cfg.get_value("controls", "button_scale", 1.0))
	handedness_slider.value = float(cfg.get_value("controls", "handedness", 0.0))
	look_zone_slider.value = float(cfg.get_value("controls", "look_zone", 0.48))
	Engine.max_fps = int(frame_slider.value)
	AudioServer.set_bus_volume_db(0, linear_to_db(max(volume_slider.value, 0.001)))

func _close() -> void:
	queue_free()
