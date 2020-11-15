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


Module mod_RENDER

    Dim show_model As Boolean = False

    Public Sub draw_scene()
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 1")
            End
        End If
        If old_fbo_w <> G_Buffer.fbo_w Or old_fbo_h <> G_Buffer.fbo_h Then

            G_Buffer.init()
            G_Buffer.getsize(old_fbo_w, old_fbo_h)
            FBO_ALPHA_class.init()
            FBO_Final.init()

            Return
        End If

        glEnable(GL_BLEND)
        'Gl.glBlendEquationSeparate(Gl.GL_FUNC_ADD, Gl.GL_FUNC_ADD)
        'Gl.glBlendFuncSeparate(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA, Gl.GL_ONE, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        'Gl.glBlendFunc(Gl.GL_ONE, Gl.GL_ONE_MINUS_SRC_ALPHA)

        'glBlendFuncSeparate(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA, GL_ONE, GL_ZERO)
        glDisable(GL_BLEND)
        Dim h, w As Integer
        Dim x, y, z, sw As Single
        G_Buffer.getsize(w, h)
        ResizeGL(w, h)
        ViewPerspective(w, h)
        set_eyes()
        Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, model_view)

        glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, gBufferFBO)

        glClearColor(0.0!, 0.0!, 0.0!, 0.0!)
        'Dim er3 = glGetError()
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        'draw dome ======================================
        G_Buffer.attachColorTexture()
        draw_dome()
        G_Buffer.attach_Color_Normal_Position()
        'draw dome ======================================

        glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position0)
        glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, position1)
        glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, position2)

        glEnable(GL_LIGHT0)
        glEnable(GL_LIGHT1)
        glEnable(GL_LIGHT2)

        If wire_Frame Then
            glPolygonMode(GL_FRONT, GL_LINE)
        Else
            glPolygonMode(GL_FRONT, GL_FILL)
        End If



        glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)

        glDepthFunc(Gl.GL_LEQUAL)
        'glFrontFace(Gl.GL_CCW)
        glEnable(GL_CULL_FACE)
        'Dim er1 = glGetError()
        '============================== moons
        G_Buffer.attach_Color_Normal_Position()

        'glColor4f(0.4!, 0.4!, 0.4!, 1.0!)
        Dim c As Single
        glUseProgram(shader_list.genericBump_shader)
        glUniform1i(genericBump_mask, 1) '1 = lit
        glUniform1i(genericBump_color, 0)
        glUniform1i(genericBump_normal, 1)
        'glUniform3f(genericBump_camPosition, eyeX, eyeY, eyeZ)
        glActiveTexture(GL_TEXTURE0 + 0)
        Gl.glBindTexture(GL_TEXTURE_2D, phobos_color)
        glActiveTexture(GL_TEXTURE0 + 1)
        Gl.glBindTexture(GL_TEXTURE_2D, phobos_normal)
        'Dim er0 = glGetError()
        'draw large stationary moon
        glPushMatrix()
        glColor4f(0.6!, 0.6!, 0.6!, 1.0!)
        glTranslatef(-40.0!, 0.0!, 0.0!)
        glScalef(30.0!, 30.0!, 30.0!)
        Gl.glRotatef(23, 1.0!, 0.0!, 1.0!)
        Gl.glRotatef(rot_pos, 0.0!, 1.0!, 0.0!)
        'rot_pos += (game_time * 15.0)
        If rot_pos > 360.0 Then
            rot_pos -= 360.0
        End If
        'giant rotating asteroid
        glCallList(asteroid_hd)
        glPopMatrix()
        z_speed = 150.0
        Dim v As vec3
        For k = 0 To star_count
            v.x = positions(k).x
            v.y = positions(k).y
            v.z = positions(k).z
            c = 0.5 - (Abs(z) * 0.00025)
            sw = positions(k).w
            glPushMatrix()
            glTranslatef(v.x, v.y, v.z)
            glScalef(sw, sw, sw)
            Gl.glRotatef(rot_pos, 0.0!, 1.0!, 0.0!)
            'glColor4f(c, c, c, 1.0!)
            v.x += eyeX
            v.y += eyeY
            v.z += eyeZ

            v = transform(v, model_view)
            Dim dist As Single = v.z
            If dist < -250.0! Then
                glCallList(asteroid)
            Else
                glCallList(asteroid_hd)
            End If
            glPopMatrix()
            If Not pause_stars Then
                positions(k).z += (z_speed * game_time)
                If positions(k).z > star_field_size Then
                    positions(k).z = -star_field_size
                End If
            End If
        Next
        glUseProgram(0)
        'glEnable(GL_TEXTURE_2D)
        glActiveTexture(GL_TEXTURE0 + 1)
        Gl.glBindTexture(GL_TEXTURE_2D, 0)
        glActiveTexture(GL_TEXTURE0 + 0)
        Gl.glBindTexture(GL_TEXTURE_2D, 0)

        '==================================
        'draw ship in center of nav ball
        glFrontFace(GL_CCW)
        glUseProgram(0)
        glBindTexture(GL_TEXTURE_2D, 0)

        glPushMatrix()
        Gl.glScalef(0.005, 0.005, 0.005)
        glColor4f(0.4!, 0.4!, 0.4!, 1.0!)
        glCallList(chopper)
        glPopMatrix()
        'chopper body
        'G_Buffer.attachColorTexture()
        glColor4f(0.6!, 0.6!, 0.6!, 1.0!)
        glUseProgram(shader_list.LightOnlyDF_shader)
        glUniform1i(LightOnlyDF_mask, 1)
        'Gl.glBindTexture(GL_TEXTURE_2D, chopper)
        glCallList(chopper)

        '-----------------
        'Grid 
        'glPushMatrix()
        'Gl.glScalef(10.0!, 1.0!, 10.0!)
        'glCallList(grid)
        'glPopMatrix()
        '-----------------

        'chopper lights
        Dim t = blink.ElapsedMilliseconds

        glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, FBO_ALPHA)
        glClear(GL_COLOR_BUFFER_BIT)
        If t > 1200 Then
            blink.Restart()
        End If
        glDisable(GL_TEXTURE_2D)
        glBindTexture(GL_TEXTURE_2D, 0)
        glUseProgram(0)
        'copy depth buffer
        'Dim ee = glGetError()
        'glBindFramebufferEXT(GL_READ_FRAMEBUFFER_EXT, gBufferFBO)
        'glBindFramebufferEXT(GL_DRAW_FRAMEBUFFER_EXT, FBO_ALPHA) ' write to default framebuffer
        'glBlitFramebufferEXT(0, 0, w, h, 0, 0, w, h, GL_DEPTH_BUFFER_BIT, GL_NEAREST)
        'Dim eee = glGetError()
        '==========================================
        glDepthMask(GL_FALSE)
        'draw very far away items first!
        draw_galaxy()
        '========================================================================================== alpha items
        sort_alpha_items() ' sort back to front
        glPolygonMode(GL_FRONT_AND_BACK, GL_FILL)
        glEnable(GL_BLEND)
        glActiveTexture(GL_TEXTURE0)

        glUseProgram(shader_list.BBbyPointAlpha_shader)
        glUniform1f(BBbyPointAlpha_texture, 0)
        For i = 0 To alpha_order_list_count - 1
            Select Case alpha_order_list(i).object_type
                Case 1
                    If frmMain.m_show_lights.Checked Then
                        draw_space_light_disc(alpha_order_list(i).index)
                    End If
                Case 2
                    emitters(alpha_order_list(i).index).draw_particles()

            End Select
        Next

        glUseProgram(0)
        glDisable(GL_BLEND)
        glEnable(GL_DEPTH_TEST)
        glDepthMask(GL_TRUE)
        '========================================================================================== alpha items


        glUseProgram(shader_list.SolidAlphaColorOnly_shader)
        If t < 600 Then
            glColor4f(0.0!, 1.0!, 0.0!, 1.0!)
            glBindTexture(GL_TEXTURE_2D, white_tex)
            glCallList(bLight)
            glColor4f(0.1!, 0.1!, 0.1!, 1.0!)
            Gl.glBindTexture(GL_TEXTURE_2D, 0)
            glCallList(fLight)

        Else
            glColor4f(0.1!, 0.1!, 0.1!, 1.0!)
            Gl.glBindTexture(GL_TEXTURE_2D, 0)
            glCallList(bLight)
            glColor4f(1.0!, 0.0!, 0.0!, 1.0!)
            glBindTexture(GL_TEXTURE_2D, white_tex)
            glCallList(fLight)

        End If
        glUseProgram(0)
        'glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        'glClear(GL_COLOR_BUFFER_BIT Or GL_DEPTH_BUFFER_BIT)

        render_sphear_alpha()

        'glCallList(chopper)
        'Gdi.SwapBuffers(pb1_hDC)
        'Return

        'glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        'glClear(GL_COLOR_BUFFER_BIT Or GL_DEPTH_BUFFER_BIT)
        ViewOrtho()
        render_FBO(w, h)
        If frmMain.m_show_buffers.Checked Then
            draw_FBO_Buffers(w, h)
        End If
        'test_gPosition(w, h)
        '=========================
        draw_ortho_Stuff()
        'swap buffers
        Gdi.SwapBuffers(pb1_hDC)
    End Sub
    Private Sub draw_space_light_disc(ByVal index As Integer)
        'glDisable(GL_CULL_FACE)
        glUniform1f(BBbyPointAlpha_rotation, 0.0)

        glBindTexture(GL_TEXTURE_2D, transDisc)
        'glBindTexture(GL_TEXTURE_2D, boobs_texture)
        Dim sc(3) As Single
        Dim p(3) As Single
        Dim i = index * 4
        p(0) = spaceL_position(i + 0)
        p(1) = spaceL_position(i + 1)
        p(2) = spaceL_position(i + 2)

        sc(0) = spaceL_color(i + 0)
        sc(1) = spaceL_color(i + 1)
        sc(2) = spaceL_color(i + 2)


        glUniform1f(BBbyPointAlpha_size, spaceL_position(i + 3))
        glPushMatrix()
        glTranslatef(p(0), p(1), p(2))
        glColor4f(sc(0), sc(1), sc(2), 1.0)
        glBegin(GL_POINTS)
        'Gl.glVertex3f(p(0), p(1), p(2))
        glVertex3f(0.0!, 0.0!, 0.0!)
        glEnd()
        glPopMatrix()


    End Sub
    Private Sub draw_dome()
        Gl.glUseProgram(shader_list.genericBump_shader)
        glUniform1i(genericBump_color, 0)
        glUniform1i(genericBump_mask, 0)

        glBindTexture(GL_TEXTURE_2D, space_dome_image)

        glPushMatrix()
        glTranslatef(eyeX, eyeY, eyeZ)
        glDisable(GL_DEPTH_TEST)
        glCallList(space_dome)
        glBindTexture(GL_TEXTURE_2D, 0)
        glEnable(GL_DEPTH_TEST)
        glPopMatrix()

        glUseProgram(0)
    End Sub
    Private Sub draw_ortho_Stuff()
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Gl.glColor4f(0.3, 0.0, 0.0, 0.4)
        Gl.glBegin(Gl.GL_QUADS)
        Gl.glVertex3f(0.0, -frmMain.PB1.Height + 20, 0.0)
        Gl.glVertex3f(0.0, -frmMain.PB1.Height, 0.0)
        Gl.glVertex3f(frmMain.PB1.Width, -frmMain.PB1.Height, 0.0)
        Gl.glVertex3f(frmMain.PB1.Width, -frmMain.PB1.Height + 20, 0.0)
        'Gl.glColor4f(0.1, 0.0, 0.0, 0.6)
        Gl.glEnd()
        Dim str = " FPS:" + FramesPerSecond.ToString + " [ d_left " + d_left.ToString("0.0000") + " [ d_right " + d_right.ToString("0.0000")
        glutPrint(10, 8 - frmMain.PB1.Height, str.ToString, 0.0, 1.0, 0.0, 1.0)

        glutPrint(10, -30, game_time.ToString("0.00000000000"), 1.0, 1.0, 0.0, 1.0)

        Gl.glDisable(Gl.GL_BLEND)

    End Sub
    Private Sub render_FBO(ByVal w As Single, ByVal h As Single)
        Dim p As New Point(0, 0)
        glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, TEMP_FBO)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        glRenderMode(GL_FRONT)
        glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        glDisable(GL_DEPTH_TEST)
        'glColor4f(0.4!, 0.4!, 0.4!, 1.0!)

        Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        glUseProgram(shader_list.deferred_shader)

        glUniform1i(deferred_gColor, 0)
        glUniform1i(deferred_gNormal, 1)
        glUniform1i(deferred_gPosition, 2)
        glUniform1i(deferred_AlphaColor, 3)
        glUniformMatrix4fv(deferred_ModelViewMatrix, 1, 0, model_view)
        'lights
        glUniform1i(deferred_space_light_count, SPACE_LIGHT_COUNT)
        glUniform4fv(deferred_spacelight_color, SPACE_LIGHT_COUNT, spaceL_color)
        glUniform4fv(deferred_spacelight_position, SPACE_LIGHT_COUNT, spaceL_position)

        glActiveTexture(GL_TEXTURE0 + 0)
        glBindTexture(GL_TEXTURE_2D, gColor)
        glActiveTexture(GL_TEXTURE0 + 1)
        glBindTexture(GL_TEXTURE_2D, gNormal)
        glActiveTexture(GL_TEXTURE0 + 2)
        glBindTexture(GL_TEXTURE_2D, gPosition)
        glActiveTexture(GL_TEXTURE0 + 3)
        glBindTexture(GL_TEXTURE_2D, gColor_ALPHA)

        draw_main_rec(p, w, h)
        glUseProgram(0)

        glActiveTexture(GL_TEXTURE0 + 3)
        glBindTexture(GL_TEXTURE_2D, 0)
        glActiveTexture(GL_TEXTURE0 + 2)
        glBindTexture(GL_TEXTURE_2D, 0)
        glActiveTexture(GL_TEXTURE0 + 1)
        glBindTexture(GL_TEXTURE_2D, 0)
        glActiveTexture(GL_TEXTURE0 + 0)
        glBindTexture(GL_TEXTURE_2D, 0)
        glDisable(GL_TEXTURE_2D)
        '=======================================================
        'do FXAA 
        glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        glUseProgram(shader_list.FXAA_shader)
        glUniform1i(FXAA_color, 0)
        glUniform2f(FXAA_screenSize, w, h)
        glBindTexture(GL_TEXTURE_2D, FINAL_COLOR)
        'glBindTexture(GL_TEXTURE_2D, gColor)
        draw_main_rec(New Point(0, 0), w, h)
        glBindTexture(GL_TEXTURE_2D, 0)
        glUseProgram(0)
    End Sub

    Private Sub draw_FBO_Buffers(ByVal w As Integer, ByVal h As Integer)
        Dim wc As Integer = w / 2
        Dim w3 As Integer = w / 3
        Dim hc As Integer = h / 2
        Dim h3 As Integer = h / 3
        Dim p As New Point(0, 0)
        glDisable(GL_DEPTH_TEST)
        glActiveTexture(GL_TEXTURE0)
        glBindTexture(GL_TEXTURE_2D, 0)


        Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        glEnable(GL_TEXTURE_2D)
        '=============================================
        'deferred
        glUseProgram(shader_list.deferred_shader)
        glUniform1i(deferred_gColor, 0)
        glUniform1i(deferred_gNormal, 1)
        glUniform1i(deferred_gPosition, 2)
        glUniform1i(deferred_AlphaColor, 3)

        glActiveTexture(GL_TEXTURE0 + 0)
        glBindTexture(GL_TEXTURE_2D, gColor)
        glActiveTexture(GL_TEXTURE0 + 1)
        glBindTexture(GL_TEXTURE_2D, gNormal)
        glActiveTexture(GL_TEXTURE0 + 2)
        glBindTexture(GL_TEXTURE_2D, gPosition)
        glActiveTexture(GL_TEXTURE0 + 3)
        glBindTexture(GL_TEXTURE_2D, gColor_ALPHA)
        p.X = 0.0
        p.Y = 0.0
        draw_main_rec(p, wc, hc)
        glUseProgram(0)

        glActiveTexture(GL_TEXTURE0 + 3)
        glBindTexture(GL_TEXTURE_2D, 0)
        glActiveTexture(GL_TEXTURE0 + 2)
        glBindTexture(GL_TEXTURE_2D, 0)
        glActiveTexture(GL_TEXTURE0 + 1)
        glBindTexture(GL_TEXTURE_2D, 0)
        glActiveTexture(GL_TEXTURE0 + 0)
        glBindTexture(GL_TEXTURE_2D, 0)
        glDisable(GL_TEXTURE_2D)

        '=============================================
        glEnable(GL_TEXTURE_2D)
        'gNormal
        p.X = wc
        p.Y = 0.0
        glBindTexture(GL_TEXTURE_2D, gNormal)
        draw_main_rec(p, wc, hc)
        'gColor
        p.X = 0.0 : p.Y = -hc
        glBindTexture(GL_TEXTURE_2D, gColor)
        draw_main_rec(p, w3, h3)
        'Position
        p.X = w3 : p.Y = -hc
        glBindTexture(GL_TEXTURE_2D, gPosition)
        draw_main_rec(p, w3, h3)

        'blend
        p.X = w3 * 2 : p.Y = -hc
        glBindTexture(GL_TEXTURE_2D, gColor_ALPHA)
        draw_main_rec(p, w3, h3)

        'bottom blank area
        p.X = 0.0 : p.Y = -(hc + h3)
        glBindTexture(GL_TEXTURE_2D, 0)
        glColor3f(0.1, 0.1, 0.1)
        draw_main_rec(p, w, h + p.Y)

        '=============================================
        glPolygonMode(Gl.GL_FRONT, Gl.GL_LINE)
        glDisable(GL_TEXTURE_2D)
        glActiveTexture(GL_TEXTURE0)
        glBindTexture(GL_TEXTURE_2D, 0)
        '=============================================

        'deffered
        p.X = 0 : p.Y = 0
        glColor3f(1.0, 1.0, 1.0)
        draw_main_rec(p, wc, hc)
        glutPrint(p.X + 5, p.Y - 12, "-= Deferred Render =-", 1.0, 1.0, 0.0, 1.0)
        'gNormal
        p.X = wc
        glColor3f(1.0, 1.0, 1.0)
        draw_main_rec(p, wc, hc)
        glutPrint(p.X + 5, p.Y - 12, "-= gNormal Texture =-", 1.0, 1.0, 0.0, 1.0)
        'color
        p.X = 0 : p.Y = -hc
        glColor3f(1.0, 1.0, 1.0)
        draw_main_rec(p, w3, h3)
        glutPrint(p.X + 5, p.Y - 12, "-= gColor Texture =-", 1.0, 1.0, 0.0, 1.0)
        'position
        p.X = w3 : p.Y = -hc
        glColor3f(1.0, 1.0, 1.0)
        draw_main_rec(p, w3, h3)
        glutPrint(p.X + 5, p.Y - 12, "-= gPosition Texture =-", 1.0, 1.0, 0.0, 1.0)
        'gColor_ALPHA
        p.X = w3 * 2 : p.Y = -hc
        glColor3f(1.0, 1.0, 1.0)
        draw_main_rec(p, w3 * 2, h3)
        glutPrint(p.X + 5, p.Y - 12, "-= ALPHA Texture =-", 1.0, 1.0, 0.0, 1.0)
        '=============================================
        glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        ' gColor Texture  gPosition Texture 

    End Sub

    Public Sub set_eyes()

        Dim sin_x, sin_y, cos_x, cos_y As Single
        sin_x = Sin(U_Cam_X_angle + angle_offset)
        cos_x = Cos(U_Cam_X_angle + angle_offset)
        cos_y = Cos(U_Cam_Y_angle)
        sin_y = Sin(U_Cam_Y_angle)
        cam_y = Sin(U_Cam_Y_angle) * view_radius
        cam_x = (sin_x - (1 - cos_y) * sin_x) * view_radius
        cam_z = (cos_x - (1 - cos_y) * cos_x) * view_radius

        Glu.gluLookAt(cam_x + U_look_point_x, cam_y + U_look_point_y, cam_z + U_look_point_z, _
                            U_look_point_x, U_look_point_y, U_look_point_z, 0.0F, 1.0F, 0.0F)

        eyeX = cam_x + U_look_point_x
        eyeY = cam_y + U_look_point_y
        eyeZ = cam_z + U_look_point_z

    End Sub

    Private Sub draw_main_rec(ByVal p As Point, ByVal w As Integer, ByVal h As Integer)
        Gl.glBegin(Gl.GL_QUADS)
        'G_Buffer.getsize(w, h)
        '  CW...
        '  1 ------ 4
        '  |        |
        '  |        |
        '  2 ------ 3
        '

        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y - h)


        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glEnd()


    End Sub

    Dim bb_rot As Single
    Public Sub draw_test_billboard(ByVal w As Integer, ByVal h As Integer)

        Dim p As vec3
        Dim s As vec2
        Dim aref As Single = 0.1
        glPointSize(4.0!)
        bb_rot += 0.015
        If bb_rot > 2 * PI Then
            bb_rot -= 2 * PI
        End If
        Dim scale As Single = 150.0!
        s.y = scale
        s.x = scale
        s.x -= s.x / 2.0!
        s.y -= s.y / 2.0!
        p.x = Cos(bb_rot) * 80.0!
        p.y = Sin(bb_rot) * 80.0!
        p.z = 50.0!
        glPushMatrix()
        glTranslatef(p.x, p.y, p.z)

        Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        glEnable(GL_TEXTURE_2D)
        'glPolygonMode(GL_FRONT_AND_BACK, GL_FILL)
        'glDisable(GL_CULL_FACE)
        glActiveTexture(GL_TEXTURE0)

        'glColor3f(0.6!, 0.6!, 0.6!)
        glUseProgram(shader_list.BillBoardBasic_shader)
        glUniform1i(BBB_mask, 0) '0 = non-lit
        glUniform1i(BBB_color, 0)
        glUniform1f(BBB_alpharef, aref)

        glBindTexture(GL_TEXTURE_2D, boobs_texture)
        Dim miplevel As Integer = 0
        glGetTexLevelParameteriv(GL_TEXTURE_2D, miplevel, GL_TEXTURE_WIDTH, w)
        glGetTexLevelParameteriv(GL_TEXTURE_2D, miplevel, GL_TEXTURE_HEIGHT, h)
        glUniform2f(BBB_TextureSize, h, w)
        Dim aspect As Single = h / w
        s.y *= aspect
        Gl.glBegin(Gl.GL_QUADS)
        'Gl.glBegin(Gl.GL_POINTS)
        '  CW...
        '  1 ------ 4
        '  |        |
        '  |        |
        '  2 ------ 3
        '

        Gl.glMultiTexCoord2f(0, 0.00001!, 0.999!)
        Gl.glMultiTexCoord3f(1, -s.x, +s.y, 0.0!)
        glNormal3f(0.0, 0.0, 1.0!)
        Gl.glVertex3f(0.0!, 0.0!, 0.0!)

        Gl.glMultiTexCoord2f(0, 0.00001!, 0.00001!)
        Gl.glMultiTexCoord3f(1, -s.x, -s.y, 0.0!)
        Gl.glVertex3f(0.0!, 0.0!, 0.0!)


        Gl.glMultiTexCoord2f(0, 0.999!, 0.00001!)
        Gl.glMultiTexCoord3f(1, s.x, -s.y, 0.0!)
        Gl.glVertex3f(0.0!, 0.0!, 0.0!)

        Gl.glMultiTexCoord2f(0, 0.999!, 0.999!)
        Gl.glMultiTexCoord3f(1, s.x, +s.y, 0.0!)
        Gl.glVertex3f(0.0!, 0.0!, 0.0!)

        Gl.glEnd()
        glBindTexture(GL_TEXTURE_2D, 0)
        glDisable(GL_TEXTURE_2D)

        glPopMatrix()

        glUseProgram(0)
    End Sub
    Public Sub draw_light_disc()

    End Sub
    Public Sub draw_galaxy()

        Dim p As vec3

        Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)

        glUseProgram(shader_list.BBbyPointAlpha_shader)
        glUniform1i(BBbyPointAlpha_texture, 0)

        glUniform1f(BBbyPointAlpha_rotation, 0.0!)
        glUniform1f(BBbyPointAlpha_size, 900.0!)


        glColor4f(1.0, 1.0, 1.0, 1.0)

        glActiveTexture(GL_TEXTURE0 + 0)
        glBindTexture(GL_TEXTURE_2D, galaxy_texture)

        'draw point at galaxy location
        p.x = 2000.0!
        p.y = 0.0!
        p.z = 2000.0!
        glPushMatrix()
        glTranslatef(p.x + eyeX, p.y + eyeY, p.z + eyeZ)

        glBegin(GL_POINTS)
        Gl.glVertex3f(0.0!, 0.0!, 0.0!)
        Gl.glEnd()

        glPopMatrix()
        glUseProgram(0)

        glBindTexture(GL_TEXTURE_2D, 0)

    End Sub

    Public Sub render_sphear_alpha()

        'draw back of navball
        glEnable(GL_BLEND)
        glUseProgram(shader_list.SphereicAlpha_shader)
        glActiveTexture(GL_TEXTURE0)
        glUniform1i(SolidAlphaTextured_color, 0)
        glUniform3f(SolidAlphaTextured_camPos, eyeX, eyeY, eyeZ)
        glUniformMatrix4fv(SolidAlphaTextured_matrix, 1, 0, model_view)
        glBindTexture(GL_TEXTURE_2D, navBall_texture)
        'glBindTexture(GL_TEXTURE_2D, phobos_color)

        glPushMatrix()
        glRotatef(sphere_rotation_Y, 0.0, 1.0, 0.0)

        glFrontFace(GL_CW)
        Gl.glPolygonMode(GL_BACK, Gl.GL_FILL)
        glCallList(asteroid_hd)

        'draw front of navball
        glFrontFace(GL_CCW)
        Gl.glPolygonMode(GL_FRONT, Gl.GL_FILL)
        glCallList(asteroid_hd)

        glPopMatrix()

        glBindTexture(GL_TEXTURE_2D, 0)

        glUseProgram(0)
        glDisable(GL_BLEND)
    End Sub
End Module
