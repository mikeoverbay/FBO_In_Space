Module mod_Globals

    Public _STARTED As Boolean = False

    '======================================================
    'screen related variables
    Public far_clip As Single = 4000.0!
    Public U_Cam_X_angle, U_Cam_Y_angle, Cam_X_angle, Cam_Y_angle As Single
    Public look_point_x, look_point_y, look_point_z As Single
    Public U_look_point_x, U_look_point_y, U_look_point_z As Single
    Public angle_offset, u_View_Radius As Single
    Public view_radius As Single
    Public cam_x, cam_y, cam_z As Single
    Public eyeX, eyeY, eyeZ As Single
    Public screen_avg_counter, screen_draw_time, FramesPerSecond As Integer
    Public game_time As Double
    '======================================================
    '======================================================
    'direction variables
    Public d_left, d_right, d_up, d_down As Single
    Public d_left_p, d_right_p, d_up_p, d_down_p As Single
    Public l_down, r_down, u_down, b_down As Boolean
    Public l_up, r_up, u_up, b_up As Boolean
    Public sphere_rotation_Y As Single

    'render_main variables
    '======================================================
    Public wire_Frame As Boolean = False
    Public update_thread As New System.Threading.Thread(AddressOf frmMain.game_loop)
    Public direction_update_thread As New System.Threading.Thread(AddressOf frmMain.direction_control_thread)
    Public gl_busy As Boolean
    Public mouse As vec2
    '======================================================
    'texture variable holders
    Public space_dome As Integer
    Public space_dome_image As Integer
    Public phobos_color, phobos_normal, transDisc As Integer
    Public asteroid_hd, asteroid As Integer
    Public boobs_texture As Integer
    Public galaxy_texture As Integer
    Public navBall_texture As Integer
    Public chopper As Integer
    Public chop_tex As Integer
    Public fLight As Integer
    Public bLight As Integer
    Public white_tex As Integer
    Public old_fbo_w, old_fbo_h As Integer
    Public blink As New Stopwatch
    Public pause_stars As Boolean
    Public z_speed As Single = 10.0!
    Public model_view(16) As Single
    Public prospective_view(16) As Single
    Public rot_pos As Single = 0
    Public t_velocity As Single
    Public e_time As Long
    Public e_timer As New Stopwatch
    '=========================================
    Public SPACE_LIGHT_COUNT As Integer = 16
    Public STAR_COUNT As Integer = 500
    Public positions(star_count + 1) As vec4
    Public star_size As Single = 30.0!
    Public EMITTER_COUNT As Integer = 100
    Public EMITTER_SCALE As Integer = 100
    '=========================================

    '======================================================
    Public star_field_size As Single = 1000.0!
    '======================================================

    'mouse control
    Public M_DOWN As Boolean
    Public Z_MOVE As Boolean
    Public MOVE_MOD As Boolean
    Public MOVE_CAM_Z As Boolean
    '======================================================
    'Math variables
    Public Structure vec4
        Public x, y, z, w As Single
    End Structure
    Public Structure vec3
        Public x, y, z As Single
    End Structure
    Public Structure vec2
        Public x, y As Single
    End Structure
    Public Structure _indice
        Public a, b, c As Integer
    End Structure
    '======================================================
    Public smoke_testures(91) As Integer
    Public explosion_1_testures(91) As Integer
    '======================================================


End Module
