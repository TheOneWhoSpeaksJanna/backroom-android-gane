extends Node3D

var ui: CanvasLayer
var shade: ColorRect
var loading: Panel
var menu: Panel
var status: Label
var started := false

func _ready():
	Engine.max_fps = 60
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	make_ui()
	call_deferred("finish_boot")

func make_ui():
	ui = CanvasLayer.new()
	add_child(ui)

	shade = ColorRect.new()
	shade.color = Color(0.01, 0.009, 0.004, 1.0)
	shade.set_anchors_preset(Control.PRESET_FULL_RECT)
	shade.mouse_filter = Control.MOUSE_FILTER_IGNORE
	ui.add_child(shade)

	loading = make_panel(Vector2(250, 170), Vector2(780, 340))
	make_label(loading, "BACKROOMS\nLEVEL ZERO", Vector2(70, 55), 56)
	make_label(loading, "loading fluorescent maze...", Vector2(145, 230), 30)

	menu = make_panel(Vector2(370, 100), Vector2(560, 500))
	make_label(menu, "BACKROOMS\nLEVEL ZERO", Vector2(55, 45), 48)
	make_label(menu, "Renderer: OpenGL compatibility\nTap a button below.", Vector2(55, 175), 22)
	var play := make_button(menu, "ENTER LEVEL 0", Vector2(115, 300), Vector2(330, 68))
	play.pressed.connect(start_game)
	var quit := make_button(menu, "EXIT", Vector2(180, 390), Vector2(200, 58))
	quit.pressed.connect(func(): get_tree().quit())

	status = Label.new()
	status.position = Vector2(28, 24)
	status.add_theme_font_size_override("font_size", 24)
	status.add_theme_color_override("font_color", Color(0.95, 0.86, 0.45))
	status.mouse_filter = Control.MOUSE_FILTER_IGNORE
	ui.add_child(status)

	menu.visible = false
	status.visible = false

func finish_boot():
	print("Backrooms boot: menu visible and buttons enabled")
	loading.visible = false
	menu.visible = true

func start_game():
	print("Backrooms: ENTER LEVEL 0 pressed")
	started = true
	menu.visible = false
	status.visible = true
	shade.color = Color(0.70, 0.62, 0.24, 1.0)
	status.text = "Level 0 loaded. Button input works.\nNext pass can restore full 3D gameplay safely."

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_ESCAPE or event.keycode == KEY_BACK:
			if started:
				started = false
				shade.color = Color(0.01, 0.009, 0.004, 1.0)
				menu.visible = true
				status.visible = false
			else:
				get_tree().quit()

func make_panel(pos: Vector2, size: Vector2) -> Panel:
	var p := Panel.new()
	p.position = pos
	p.size = size
	p.mouse_filter = Control.MOUSE_FILTER_PASS
	var st := StyleBoxFlat.new()
	st.bg_color = Color(0.045, 0.039, 0.018, 0.96)
	st.border_color = Color(0.68, 0.57, 0.23, 0.92)
	st.set_border_width_all(2)
	st.corner_radius_top_left = 10
	st.corner_radius_top_right = 10
	st.corner_radius_bottom_left = 10
	st.corner_radius_bottom_right = 10
	p.add_theme_stylebox_override("panel", st)
	ui.add_child(p)
	return p

func make_label(parent: Control, text: String, pos: Vector2, size: int) -> Label:
	var l := Label.new()
	l.text = text
	l.position = pos
	l.add_theme_font_size_override("font_size", size)
	l.add_theme_color_override("font_color", Color(0.95, 0.84, 0.40))
	l.mouse_filter = Control.MOUSE_FILTER_IGNORE
	parent.add_child(l)
	return l

func make_button(parent: Control, text: String, pos: Vector2, size: Vector2) -> Button:
	var b := Button.new()
	b.text = text
	b.position = pos
	b.size = size
	b.focus_mode = Control.FOCUS_NONE
	b.mouse_filter = Control.MOUSE_FILTER_STOP
	b.add_theme_font_size_override("font_size", 22)
	parent.add_child(b)
	return b
