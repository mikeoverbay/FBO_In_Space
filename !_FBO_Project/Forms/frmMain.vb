#Region "imports"
Imports System.Windows
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Web
Imports Tao.OpenGl
Imports Tao.OpenGl.Gl
Imports Tao.Platform.Windows
Imports Tao.FreeGlut
Imports Tao.FreeGlut.Glut
Imports Microsoft.VisualBasic.Strings
Imports System.Math
Imports System.Object
Imports System.Threading
Imports System.Data
Imports Tao.DevIl
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic


#End Region

Public Class frmMain


    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        update_thread.Abort()
        While update_thread.IsAlive
            Application.DoEvents()
        End While
        DisableOpenGL()
    End Sub

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Space Then
            If pause_stars Then
                pause_stars = False
            Else
                pause_stars = True
            End If
        End If
        '======================================
        'view
        If e.KeyCode = 16 Then
            MOVE_MOD = True
        End If
        If e.KeyCode = 17 Then
            Z_MOVE = True
        End If
        '======================================
        'direction
        If e.KeyCode = Keys.A Then
            l_down = True
            l_up = False
        End If
        If e.KeyCode = Keys.D Then
            r_down = True
            r_up = False
        End If
        If e.KeyCode = Keys.W Then
            u_down = True
            u_up = False
        End If
        If e.KeyCode = Keys.X Then
            d_down = True
            d_up = False
        End If
        '======================================
        If e.KeyCode = Keys.L Then
            make_test_lights()
        End If
    End Sub

    Private Sub frmMain_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        '======================================
        'direction
        If e.KeyCode = Keys.A Then
            l_up = True
        End If
        If e.KeyCode = Keys.D Then
            r_up = True
        End If
        If e.KeyCode = Keys.W Then
            u_up = True
        End If
        If e.KeyCode = Keys.X Then
            d_up = True
        End If
        '======================================
        Z_MOVE = False
        MOVE_MOD = False
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim nonInvariantCulture As Globalization.CultureInfo = New Globalization.CultureInfo("en-US")
        nonInvariantCulture.NumberFormat.NumberDecimalSeparator = "."
        Thread.CurrentThread.CurrentCulture = nonInvariantCulture
        Me.Show()

        cam_x = 0
        cam_y = 0
        cam_z = 10
        look_point_x = 0
        look_point_y = 0
        look_point_z = 0
        Cam_X_angle = (PI * 0.25) + PI
        Cam_Y_angle = -PI * 0.25
        view_radius = -1.0
        '==================================
        '==================================
        'initize opengl
        Il.ilInit()
        Ilu.iluInit()
        Ilut.ilutInit()
        EnableOpenGL()
        make_shaders()
        set_shader_variables()
        '==================================
        '==================================
        'load game data
        load_game_data()
        make_xy_grid()
        make_test()
        make_test_lights()
        make_emitter_test()
        '==================================
        build_alpha_object_list()
        '==================================
        Me.KeyPreview = True    'so i catch keyboard before despatching it

        _STARTED = True
        Startup_Timer.Enabled = True
        blink.Start()
    End Sub
    Private Sub load_game_data()
        Dim sp = Application.StartupPath

        space_dome = get_X_model(sp + "\game_data\environment\dome.x")
        space_dome_image = load_img_file(sp + "\game_data\environment\space_Map.png")

        phobos_color = load_img_file(sp + "\game_data\environment\phobosmirror.png")
        'phobos_color = load_img_file(sp + "\game_data\environment\test.png")
        phobos_normal = load_img_file(sp + "\game_data\environment\phobosmirror_NORM.png")

        asteroid = get_X_model(sp + "\game_data\environment\asteroid.x")
        asteroid_hd = get_X_model(sp + "\game_data\environment\asteroid_HD.x")

        chop_tex = load_img_file(sp + "\game_data\environment\chopper_texture.png")
        chopper = get_X_model(sp + "\game_data\environment\chopper.x")

        fLight = get_X_model(sp + "\game_data\environment\front_lights.x")
        bLight = get_X_model(sp + "\game_data\environment\back_lights.x")
        white_tex = load_img_file(sp + "\game_data\environment\white.png")

        boobs_texture = load_img_file(sp + "\game_data\environment\boobs.png")
        galaxy_texture = load_img_file(sp + "\game_data\environment\galaxy_1.png")
        'navBall_texture = load_img_file(sp + "\game_data\environment\navBall.png")
        navBall_texture = load_img_file(sp + "\game_data\environment\navBall.png")

        transDisc = load_img_file(sp + "\game_data\environment\transDisc.png")
        load_particle_textures(sp)
    End Sub
    Private Sub load_particle_textures(ByVal sp As String)
        Dim sm = sp + "\game_data\effects\"
        Dim em = sp + "\game_data\effects\"

        For i = 0 To 90
            Dim ep As String = "smoke_anima\00" + i.ToString("00") + ".png"
            smoke_testures(i) = load_img_file(sm + ep)
        Next
        For i = 0 To 90
            Dim ep As String = "explosion\explosion 1_rgb00" + i.ToString("00") + ".png"
            explosion_1_testures(i) = load_img_file(em + ep)
        Next
    End Sub
    Dim elist() = {True, False}
    Dim pi2 = PI * 2.0
    Dim eRotate() = {pi2 * 0.25, pi2 * 0.5, pi2 * 0.75, pi2}
    Private Sub make_emitter_test()
        Dim rnd As New Random
        Dim scale As Single = 1000.0
        For i = 0 To emitter_count - 1
            emitters(i) = New smoke_
            emitters(i).setScale(rnd.NextDouble * EMITTER_SCALE)
            emitters(i).setScale(100.0)
            emitters(i).location.x = CSng(rnd.NextDouble - 0.5) * scale
            emitters(i).location.y = CSng(rnd.NextDouble - 0.5) * scale
            emitters(i).location.z = CSng(rnd.NextDouble - 0.5) * scale
            emitters(i).set_start_frame(CInt(rnd.NextDouble * 89)) 'random frame position
            emitters(i).set_e_type(4)
            emitters(i).enable(elist(CInt(rnd.NextDouble)))
            emitters(i).enable(True)
            emitters(i).set_rotation(pi2 * rnd.NextDouble)
            emitters(i).enable_rotation(False)
            emitters(i).start()
        Next
    End Sub

    Public Sub make_test()
        Dim rand As New Random
        Dim x, y, z, w As Single
        Dim scale As Single = star_field_size * 2
        For k = 0 To star_count
            x = CSng(rand.NextDouble - 0.5) * scale
            y = CSng(rand.NextDouble - 0.5) * scale
            z = CSng(rand.NextDouble - 0.5) * scale
            w = CSng(rand.NextDouble + 0.5) * star_size
            positions(k).x = x
            positions(k).y = y
            positions(k).z = z
            positions(k).w = w
            glPushMatrix()
            glTranslatef(x, y, z)
            glCallList(asteroid)
            glPopMatrix()
        Next
    End Sub


    Private Sub make_test_lights()
        Dim rnd = New Random
        Dim c As Single = 0
        Dim spec As Single = 0.5!
        Dim range As Single = 400.0!
        Dim scale As Single = 1500.0!
        Dim sp_light_cnt As Integer = SPACE_LIGHT_COUNT
        Dim cnt As Integer = 0

        For i = 0 To (sp_light_cnt - 1) * 4 Step 4
            ' For i = 0 To (max_space_lights - 1) * 4 Step 4
            'position
            c = (CSng(rnd.NextDouble) - 0.5) * scale
            spaceL_position(i + 0) = c
            c = (CSng(rnd.NextDouble) - 0.5) * scale
            spaceL_position(i + 1) = c
            c = (CSng(rnd.NextDouble) - 0.5) * scale
            spaceL_position(i + 2) = c
            spaceL_position(i + 3) = range
            'color
            Select Case cnt Mod 3
                Case 0
                    spaceL_color(i + 0) = cr(0)
                    spaceL_color(i + 1) = cr(1)
                    spaceL_color(i + 2) = cr(2)
                    spaceL_color(i + 3) = spec
                Case 1
                    spaceL_color(i + 0) = cg(0)
                    spaceL_color(i + 1) = cg(1)
                    spaceL_color(i + 2) = cg(2)
                    spaceL_color(i + 3) = spec
                Case 2
                    spaceL_color(i + 0) = cb(0)
                    spaceL_color(i + 1) = cb(1)
                    spaceL_color(i + 2) = cb(2)
                    spaceL_color(i + 3) = spec

            End Select
            'spaceL_color(i + 0) = cy(0)
            'spaceL_color(i + 1) = cy(1)
            'spaceL_color(i + 2) = cy(2)
            'spaceL_color(i + 3) = spec

            GoTo fuckit
            c = (CSng(rnd.NextDouble))
            spaceL_color(i + 0) = c
            c = (CSng(rnd.NextDouble))
            spaceL_color(i + 1) = c
            c = (CSng(rnd.NextDouble))
            spaceL_color(i + 2) = c
            spaceL_color(i + 3) = spec
fuckit:
            cnt += 1
            SPACE_LIGHT_COUNT = cnt
        Next
        spaceL_position(0) = 2000.0!
        spaceL_position(1) = 0.0!
        spaceL_position(2) = 2000.0!
        spaceL_position(3) = 4000.0!

        spaceL_color(0) = 1.0!
        spaceL_color(1) = 1.0!
        spaceL_color(2) = 1.0!
        spaceL_color(3) = 0.0!

    End Sub

    Public Sub get_pb1_size(ByRef x As Single, ByRef y As Single)
        x = PB1.Width
        y = PB1.Height
        Return
    End Sub

#Region "Screen_Update_Timing"
    Public Sub direction_control_thread()
        Dim clock As New Stopwatch
        Dim max_speed As Single = 0.05
        Dim t_step As Single = 0.03
        Dim HP As Single = CSng(PI / 2.0)
        clock.Start()
        While _STARTED
            Dim e = clock.ElapsedMilliseconds
            If e > 10 Then
                clock.Restart()
                '-----------------------
                'left event
                If Not l_down And Not l_up And d_left = 1.0 Then
                    l_up = True
                End If

                If l_down Then
                    r_down = False
                    If d_right = 0.0! Then
                        d_left_p += t_step
                        If d_left_p >= HP Then
                            d_left_p = HP
                        End If
                        d_left = Sin(d_left_p)
                    End If
                End If
                If l_up Then
                    l_down = False
                    If d_left_p > 0.0! Then
                        d_left_p -= t_step
                        If d_left_p < 0.0! Then
                            d_left_p = 0.0!
                            l_up = False
                        End If
                        d_left = Sin(d_left_p)
                    Else
                        d_left = 0.0!
                        l_up = False
                    End If
                End If
                '-----------------------
                'right event
                If Not r_down And Not r_up And d_right = 1.0 Then
                    r_up = True
                End If
                If r_down Then
                    l_down = False
                    If d_left = 0.0! Then
                        d_right_p += t_step
                        If d_right_p >= HP Then
                            d_right_p = HP
                        End If
                        d_right = Sin(d_right_p)
                    End If
                End If
                If r_up Then
                    r_down = False
                    If d_right_p > 0.0! Then
                        d_right_p -= t_step
                        If d_right_p < 0.0! Then
                            d_right_p = 0.0!
                            r_up = False
                        End If
                        d_right = Sin(d_right_p)
                    Else
                        d_right = 0.0!
                        r_up = False

                    End If
                End If
            End If
skip:

            Dim s As Single = 0.2
            sphere_rotation_Y += (d_left * s)
            sphere_rotation_Y -= (d_right * s)
            If sphere_rotation_Y > 360.0! Then
                sphere_rotation_Y -= 360.0!
            End If
            If sphere_rotation_Y < 0.0! Then
                sphere_rotation_Y += 360.0!
            End If
            Thread.Sleep(3)
        End While

    End Sub


    Public Function need_update() As Boolean
        'This updates the display if the mouse has changed the view angles, locations or distance.
        Dim update As Boolean = False

        If look_point_x <> U_look_point_x Then
            U_look_point_x = look_point_x
            update = True
        End If
        If look_point_y <> U_look_point_y Then
            U_look_point_y = look_point_y
            update = True
        End If
        If look_point_z <> U_look_point_z Then
            U_look_point_z = look_point_z
            update = True
        End If
        If Cam_X_angle <> U_Cam_X_angle Then
            U_Cam_X_angle = Cam_X_angle
            update = True
        End If
        If Cam_Y_angle <> U_Cam_Y_angle Then
            U_Cam_Y_angle = Cam_Y_angle
            update = True
        End If
        If view_radius <> u_View_Radius Then
            u_View_Radius = view_radius
            update = True
        End If
        If stop_updating And update Then update_screen()

        Return update
    End Function

    Public Sub game_loop()

        Dim old_time As Long = 0
        Dim new_time As Long = 0
        Dim elapsed_time As Long = 0
        Dim freq = Stopwatch.Frequency
        FramesPerSecond = 10.0
        Dim swat As New Stopwatch
        Dim game_stopwatch As New Stopwatch
        'Dim x, z As Single
        Dim s As Single = 2.0
        'l_timer.Restart()
        swat.Start()
        game_stopwatch.Start()
        old_time = swat.ElapsedTicks
        While _STARTED
            old_time = new_time
            new_time = game_stopwatch.ElapsedTicks
            need_update()
            angle_offset = 0

            Application.DoEvents()
            If Not stop_updating And Not gl_busy And Not Me.WindowState = FormWindowState.Minimized Then

                'If Not w_changing And Not stop_updating Then
                '    If spin_light And l_timer.ElapsedMilliseconds > 32 Then
                '        l_timer.Restart()
                '        l_rot += 0.015
                '        If l_rot > 2 * PI Then
                '            l_rot -= (2 * PI)
                '        End If
                '        sun_angle = l_rot
                '        x = Cos(l_rot) * (sun_radius * s)
                '        z = Sin(l_rot) * (sun_radius * s)

                '        position0(0) = x
                '        position0(1) = 10.0
                '        position0(2) = z

                '    End If

                e_time = e_timer.ElapsedMilliseconds
                update_screen()

                e_timer.Restart()
                screen_draw_time = CInt(swat.ElapsedMilliseconds)
                If screen_draw_time >= 1000 Then
                    FramesPerSecond = screen_avg_counter
                    screen_avg_counter = 0.0
                    swat.Restart()
                Else
                End If
            End If
            Thread.Sleep(3)
            elapsed_time = game_stopwatch.ElapsedTicks - new_time
            game_time = elapsed_time / Stopwatch.Frequency
            'game_stopwatch.Restart()
            'Application.DoEvents()
        End While
        'Thread.CurrentThread.Abort()
    End Sub
    Private Delegate Sub update_screen_delegate()
    Private Sub update_screen()
        gl_busy = True
        Try
            If Me.InvokeRequired Then
                Me.Invoke(New update_screen_delegate(AddressOf update_screen))
            Else
                draw_scene()
                screen_avg_counter += 1.0
            End If
        Catch ex As Exception

        End Try
        gl_busy = False
    End Sub

    Private Sub Startup_Timer_Tick(sender As Object, e As EventArgs) Handles Startup_Timer.Tick
        Startup_Timer.Enabled = False
        '==========================
        update_thread.IsBackground = True
        update_thread.Name = "mouse updater"
        update_thread.Priority = ThreadPriority.Normal
        update_thread.Start()
        '==========================
        direction_update_thread.IsBackground = True
        direction_update_thread.Name = "Dirction Control"
        direction_update_thread.Priority = ThreadPriority.Normal
        direction_update_thread.Start()

    End Sub

#End Region

#Region "PB1 Mouse"

    Private Sub PB1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles PB1.MouseDoubleClick
        'Return
        If Me.FormBorderStyle = Forms.FormBorderStyle.None Then
            Me.FormBorderStyle = Forms.FormBorderStyle.Sizable
            MM.Visible = True
        Else
            Me.FormBorderStyle = Forms.FormBorderStyle.None
            MM.Visible = False
        End If
    End Sub
    Private Sub PB1_MouseDown(sender As Object, e As MouseEventArgs) Handles PB1.MouseDown
        If e.Button = Forms.MouseButtons.Left Then
            M_DOWN = True
        End If
        If e.Button = Forms.MouseButtons.Right Then
            MOVE_CAM_Z = True
        End If

    End Sub

    Private Sub PB1_MouseMove(sender As Object, e As MouseEventArgs) Handles PB1.MouseMove
        Dim dead As Integer = 1
        Dim t As Single
        Dim M_Speed As Single = 0.8
        Dim zoom_out_limit As Single = -4000.0!
        Dim ms As Single = 0.2F * view_radius ' distance away changes speed.. THIS WORKS WELL!
        If M_DOWN Then
            If e.X > (mouse.x + dead) Then
                If e.X - mouse.x > 100 Then t = (1.0F * M_Speed)
            Else : t = CSng(Sin((e.X - mouse.x) / 100)) * M_Speed
                If Not Z_MOVE Then
                    If MOVE_MOD Then ' check for modifying flag
                        look_point_x -= ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_z -= ((t * ms) * (-Sin(Cam_X_angle)))
                    Else
                        Cam_X_angle -= t
                    End If
                    If Cam_X_angle > (2 * PI) Then Cam_X_angle -= (2 * PI)
                    mouse.x = e.X
                End If
            End If
            If e.X < (mouse.x - dead) Then
                If mouse.x - e.X > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.x - e.X) / 100)) * M_Speed
                If Not Z_MOVE Then
                    If MOVE_MOD Then ' check for modifying flag
                        look_point_x += ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_z += ((t * ms) * (-Sin(Cam_X_angle)))
                    Else
                        Cam_X_angle += t
                    End If
                    If Cam_X_angle < 0 Then Cam_X_angle += (2 * PI)
                    mouse.x = e.X
                End If
            End If
            ' ------- Y moves ----------------------------------
            If e.Y > (mouse.y + dead) Then
                If e.Y - mouse.y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((e.Y - mouse.y) / 100)) * M_Speed
                If Z_MOVE Then
                    look_point_y -= (t * ms)
                Else
                    If MOVE_MOD Then ' check for modifying flag
                        look_point_z -= ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_x -= ((t * ms) * (Sin(Cam_X_angle)))
                    Else
                        Cam_Y_angle -= t
                    End If
                    If Cam_Y_angle < -PI / 2.0 Then Cam_Y_angle = -PI / 2.0 + 0.001
                End If
                mouse.y = e.Y
            End If
            If e.Y < (mouse.y - dead) Then
                If mouse.y - e.Y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.y - e.Y) / 100)) * M_Speed
                If Z_MOVE Then
                    look_point_y += (t * ms)
                Else
                    If MOVE_MOD Then ' check for modifying flag
                        look_point_z += ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_x += ((t * ms) * (Sin(Cam_X_angle)))
                    Else
                        Cam_Y_angle += t
                    End If
                    If Cam_Y_angle > 1.3 Then Cam_Y_angle = 1.3
                End If
                mouse.y = e.Y
            End If
            Return
        End If
        If MOVE_CAM_Z Then
            If e.Y > (mouse.y + dead) Then
                If e.Y - mouse.y > 100 Then t = (10)
            Else : t = CSng(Sin((e.Y - mouse.y) / 100)) * 12
                If view_radius + (t * (view_radius * 0.2)) >= zoom_out_limit Then
                    view_radius += (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                    mouse.y = e.Y
                Else
                    view_radius = zoom_out_limit
                    mouse.y = e.Y
                    Return
                End If
            End If
            If e.Y < (mouse.y - dead) Then
                If mouse.y - e.Y > 100 Then t = (10)
            Else : t = CSng(Sin((mouse.y - e.Y) / 100)) * 12
                view_radius -= (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                If view_radius > -0.01 Then view_radius = -0.01
                mouse.y = e.Y
            End If
            If view_radius > -0.1 Then view_radius = -0.1
            Return
        End If
        mouse.x = e.X
        mouse.y = e.Y
    End Sub

    Private Sub PB1_MouseUp(sender As Object, e As MouseEventArgs) Handles PB1.MouseUp
        M_DOWN = False
        MOVE_CAM_Z = False
        MOVE_MOD = False
    End Sub

#End Region

#Region "MM menu events"

    Private Sub m_edit_shaders_Click(sender As Object, e As EventArgs) Handles m_edit_shaders.Click
        frmShaderEditor.Show()
    End Sub

#End Region


    Private Sub m_wire_frame_Click(sender As Object, e As EventArgs) Handles m_wire_frame.Click
        wire_Frame = m_wire_frame.Checked
    End Sub
End Class
