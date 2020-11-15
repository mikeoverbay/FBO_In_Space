Imports System
Imports System.Text
Imports System.String
Imports System.IO
Imports Tao.OpenGl.Glu
Imports Tao.OpenGl.Gl
Imports Tao.Platform.Windows
Module shader_loader

    Public shader_list As New shader_list_
    Public Class shader_list_
        Public BillBoardBasic_shader As Integer
        Public BillBoardBasicAlpha_shader As Integer
        Public FXAA_shader As Integer
        Public genericBump_shader As Integer
        Public deferred_shader As Integer
        Public LightOnlyDF_shader As Integer
        Public passthrough_shader As Integer
        Public SphereicAlpha_shader As Integer
        Public SolidAlphaTextured_shader As Integer
        Public SolidAlphaColorOnly_shader As Integer
        Public BBbyPointAlpha_shader As Integer
    End Class

#Region "variables"
    Public stop_updating As Boolean
    Public shaders As shaders__
    Public Structure shaders__
        Public shader() As shaders_
        Public Function f(ByVal name As String) As Integer
            For Each s In shader
                If s.shader_name = name Then
                    Return s.shader_id
                End If
            Next
            Return 0
        End Function
    End Structure
    Public Structure shaders_
        Public fragment As String
        Public vertex As String
        Public geo As String
        Public shader_name As String
        Public shader_id As Integer
        Public has_geo As Boolean
        Public Sub set_call_id(ByVal id As Integer)
            Try
                CallByName(shader_list, Me.shader_name, CallType.Set, Me.shader_id)
            Catch ex As Exception
                MsgBox("missing member from shader_list:" + Me.shader_name, MsgBoxStyle.Exclamation, "Oops!")
                End
            End Try
        End Sub
    End Structure

#End Region


    Public Sub make_shaders()
        'I'm tierd of all the work every time I add a shader.
        'So... Im going to automate the process.. Hey.. its a computer for fucks sake!
        Dim f_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders\", "*fragment.glsl")
        Dim v_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders\", "*vertex.glsl")
        Dim g_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders\", "*geo.glsl")
        Array.Sort(f_list)
        Array.Sort(v_list)
        Array.Sort(g_list)
        ReDim shaders.shader(f_list.Length - 1)
        With shaders

            For i = 0 To f_list.Length - 1
                .shader(i) = New shaders_
                With .shader(i)
                    Dim fn As String = Path.GetFileNameWithoutExtension(f_list(i))
                    Dim ar = fn.Split("_")
                    .shader_name = ar(0) + "_shader"
                    .fragment = f_list(i)
                    .vertex = v_list(i)
                    .geo = ""
                    For Each g In g_list ' find matching geo if there is one.. usually there wont be
                        If g.Contains(ar(0)) Then
                            .geo = g
                            .has_geo = True ' found a matching geo so we need to set this true
                        End If
                    Next
                    .shader_id = -1
                    .set_call_id(-1)
                End With
            Next

        End With
        Dim fs As String
        Dim vs As String
        Dim gs As String

        For i = 0 To shaders.shader.Length - 1
            With shaders.shader(i)
                vs = .vertex
                fs = .fragment
                gs = .geo
                Dim id = assemble_shader(vs, gs, fs, .shader_id, .shader_name, .has_geo)
                .set_call_id(id)
                .shader_id = id

                'Debug.WriteLine(.shader_name + "  Id:" + .shader_id.ToString)
            End With
        Next

    End Sub
    Public Function assemble_shader(v As String, g As String, f As String, ByRef shader As Integer, ByRef name As String, ByRef has_geo As Boolean) As Integer
        frmShaderEditor.TopMost = False
        Dim vs(1) As String
        Dim gs(1) As String
        Dim fs(1) As String
        Dim vertexObject As Integer
        Dim geoObject As Integer
        Dim fragmentObject As Integer
        Dim status_code As Integer
        Dim info As New StringBuilder
        info.Length = 8192
        Dim info_l As Integer

        If shader > 0 Then
            glUseProgram(0)
            glDeleteProgram(shader)
            glGetProgramiv(shader, gL_DELETE_STATUS, status_code)
            glFinish()
        End If

        Dim e = glGetError
        If e <> 0 Then
            Dim s = gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            'MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        'have a hard time with files remaining open.. hope this fixes it! (yep.. it did)
        Using vs_s As New StreamReader(v)
            vs(0) = vs_s.ReadToEnd
            vs_s.Close()
            vs_s.Dispose()
            vs(0) = clean_shader(vs(0)) ' remove non_ascii characters
        End Using
        Using fs_s As New StreamReader(f)
            fs(0) = fs_s.ReadToEnd
            fs_s.Close()
            fs_s.Dispose()
            fs(0) = clean_shader(fs(0))
        End Using
        If has_geo Then
            Using gs_s As New StreamReader(g)
                gs(0) = gs_s.ReadToEnd
                gs_s.Close()
                gs_s.Dispose()
                gs(0) = clean_shader(gs(0))
            End Using
        End If


        vertexObject = glCreateShader(gL_VERTEX_SHADER)
        fragmentObject = glCreateShader(gL_FRAGMENT_SHADER)
        '--------------------------------------------------------------------
        shader = glCreateProgram()

        ' Compile vertex shader
        glShaderSource(vertexObject, 1, vs, vs(0).Length)
        glCompileShader(vertexObject)
        glGetShaderInfoLog(vertexObject, 8192, info_l, info)
        glGetShaderiv(vertexObject, gL_COMPILE_STATUS, status_code)
        If Not status_code = gL_TRUE Then
            glDeleteShader(vertexObject)
            gl_error(name + "_vertex didn't compile!" + vbCrLf + info.ToString)

        End If

        e = glGetError
        If e <> 0 Then
            Dim s = gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        If has_geo Then
            'geo
            geoObject = glCreateShader(gL_GEOMETRY_SHADER_EXT)
            glShaderSource(geoObject, 1, gs, gs(0).Length)
            glCompileShader(geoObject)
            glGetShaderInfoLog(geoObject, 8192, info_l, info)
            glGetShaderiv(geoObject, gL_COMPILE_STATUS, status_code)
            If Not status_code = gL_TRUE Then
                glDeleteShader(geoObject)
                gl_error(name + "_geo didn't compile!" + vbCrLf + info.ToString)

            End If
            e = glGetError
            If e <> 0 Then
                Dim s = gluErrorString(e)
                Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
                MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
            End If
            If name.Contains("BBbyPoint") Then

                glProgramParameteriEXT(shader, GL_GEOMETRY_INPUT_TYPE_EXT, GL_POINTS)
                glProgramParameteriEXT(shader, GL_GEOMETRY_OUTPUT_TYPE_EXT, GL_TRIANGLE_STRIP)
                glProgramParameteriEXT(shader, GL_GEOMETRY_VERTICES_OUT_EXT, 4)
            End If
            If name.Contains("normal") Then
                glProgramParameteriEXT(shader, gL_GEOMETRY_INPUT_TYPE_EXT, gL_TRIANGLES)
                glProgramParameteriEXT(shader, gL_GEOMETRY_OUTPUT_TYPE_EXT, gL_LINE_STRIP)
                glProgramParameteriEXT(shader, gL_GEOMETRY_VERTICES_OUT_EXT, 18)
            End If

            e = glGetError
            If e <> 0 Then
                Dim s = gluErrorString(e)
                Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
                MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
            End If

        End If

        ' Compile fragment shader

        glShaderSource(fragmentObject, 1, fs, fs(0).Length)
        glCompileShader(fragmentObject)
        glGetShaderInfoLog(fragmentObject, 8192, info_l, info)
        glGetShaderiv(fragmentObject, gL_COMPILE_STATUS, status_code)

        If Not status_code = gL_TRUE Then
            glDeleteShader(fragmentObject)
            gl_error(name + "_fragment didn't compile!" + vbCrLf + info.ToString)

        End If
        e = glGetError
        If e <> 0 Then
            Dim s = gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        'attach shader objects
        glAttachShader(shader, fragmentObject)
        If has_geo Then
            glAttachShader(shader, geoObject)
        End If
        glAttachShader(shader, vertexObject)

        'link program
        glLinkProgram(shader)

        ' detach shader objects
        glDetachShader(shader, fragmentObject)
        If has_geo Then
            glDetachShader(shader, geoObject)
        End If
        glDetachShader(shader, vertexObject)

        e = glGetError
        If e <> 0 Then
            Dim s = gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        glGetShaderiv(shader, gL_LINK_STATUS, status_code)

        If Not status_code = gL_TRUE Then
            glDeleteProgram(shader)
            gl_error(name + " did not link!" + vbCrLf + info.ToString)

        End If

        'delete shader objects
        glDeleteShader(fragmentObject)
        glGetShaderiv(fragmentObject, gL_DELETE_STATUS, status_code)
        If has_geo Then
            glDeleteShader(geoObject)
            glGetShaderiv(geoObject, gL_DELETE_STATUS, status_code)
        End If
        glDeleteShader(vertexObject)
        glGetShaderiv(vertexObject, gL_DELETE_STATUS, status_code)
        e = glGetError
        If e <> 0 Then
            'aways throws a error after deletion even though the status shows them as deleted.. ????
            Dim s = gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            'MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        vs(0) = Nothing
        fs(0) = Nothing
        If has_geo Then
            gs(0) = Nothing
        End If
        GC.Collect()
        GC.WaitForFullGCComplete()

        frmShaderEditor.TopMost = True

        Return shader
    End Function

    Private Function clean_shader(ByRef s As String) As String
        'removes any non-ascii characters
        Dim encoder = Encoding.UTF8
        Dim ar = encoder.GetBytes(s)
        s = encoder.GetString(ar)
        Return s
    End Function
    Public Sub gl_error(s As String)
        s = s.Replace(vbLf, vbCrLf)
        s.Replace("0(", vbCrLf + "(")
        frmShaderError.Show()
        frmShaderError.er_tb.Text += s
    End Sub

    '==============================================================================================================
    'template
    Private Sub set_()

    End Sub
    '==============================================================================================================
    '==============================================================================================================
    'BillBoardBasic
    Public BBB_color, BBB_alpharef, BBB_mask, BBB_TextureSize As Integer
    Private Sub set_BillBoardBasic_Variables()
        BBB_color = glGetUniformLocation(shader_list.BillBoardBasic_shader, "colorMap")
        BBB_alpharef = glGetUniformLocation(shader_list.BillBoardBasic_shader, "alpharef")
        BBB_mask = glGetUniformLocation(shader_list.BillBoardBasic_shader, "mask")
        BBB_TextureSize = glGetUniformLocation(shader_list.BillBoardBasic_shader, "TextureSize")
    End Sub
    '==============================================================================================================
    '==============================================================================================================
    'BillBoardBasicAlpha
    Public BBBA_Color, BBBA_TextureSize, BBBA_Rotation As Integer
    Private Sub set_BillBoardBasicAlpha_Variables()
        BBBA_Color = glGetUniformLocation(shader_list.BillBoardBasicAlpha_shader, "colorMap")
        BBBA_TextureSize = glGetUniformLocation(shader_list.BillBoardBasicAlpha_shader, "TextureSize")
        BBBA_Rotation = glGetUniformLocation(shader_list.BillBoardBasicAlpha_shader, "rotation")
    End Sub
    '==============================================================================================================
    '==============================================================================================================
    'Deferred
    Public deferred_gColor, deferred_gNormal, deferred_gPosition As Integer
    Public deferred_ModelViewMatrix, deferred_AlphaColor As Integer
    Public deferred_spacelight_color, deferred_spacelight_position, deferred_space_light_count, deferred_space_light_render As Integer
    Public deferred_shiphull_light_color, deferred_shiphull_light_position, deferred_shiphull_light_count As Integer
    Public deferred_inst_light_color, deferred_inst_light_position, deferred_inst_light_count As Integer
    Private Sub set_lightingFBO_variables()
        deferred_gColor = glGetUniformLocation(shader_list.deferred_shader, "gColor")
        deferred_gNormal = glGetUniformLocation(shader_list.deferred_shader, "gNormal")
        deferred_gPosition = glGetUniformLocation(shader_list.deferred_shader, "gPosition")
        deferred_ModelViewMatrix = glGetUniformLocation(shader_list.deferred_shader, "ModelViewMatrix")
        deferred_AlphaColor = glGetUniformLocation(shader_list.deferred_shader, "AlphaColor")
        'light uniforms
        deferred_spacelight_color = glGetUniformLocation(shader_list.deferred_shader, "spaceL_color")
        deferred_spacelight_position = glGetUniformLocation(shader_list.deferred_shader, "spaceL_position")
        deferred_space_light_count = glGetUniformLocation(shader_list.deferred_shader, "spaceL_count")

        deferred_shiphull_light_color = glGetUniformLocation(shader_list.deferred_shader, "AlphaColor")
        deferred_shiphull_light_position = glGetUniformLocation(shader_list.deferred_shader, "AlphaColor")
        deferred_shiphull_light_count = glGetUniformLocation(shader_list.deferred_shader, "AlphaColor")

        deferred_inst_light_color = glGetUniformLocation(shader_list.deferred_shader, "AlphaColor")
        deferred_inst_light_position = glGetUniformLocation(shader_list.deferred_shader, "AlphaColor")
        deferred_inst_light_count = glGetUniformLocation(shader_list.deferred_shader, "AlphaColor")

    End Sub
    '==============================================================================================================
    '==============================================================================================================
    'genericBump
    Public genericBump_color, genericBump_normal, genericBump_camPosition, genericBump_mask As Integer
    Private Sub set_genericBump_variables()
        genericBump_color = glGetUniformLocation(shader_list.genericBump_shader, "colorMap")
        genericBump_normal = glGetUniformLocation(shader_list.genericBump_shader, "normalMap")
        genericBump_camPosition = glGetUniformLocation(shader_list.genericBump_shader, "camPosition")
        genericBump_mask = glGetUniformLocation(shader_list.genericBump_shader, "mask")
    End Sub
    '==============================================================================================================
    '==============================================================================================================
    'lightOnly
    Public LightOnlyDF_mask As Integer
    Private Sub set_LightOnlyDF_variables()
        LightOnlyDF_mask = glGetUniformLocation(shader_list.LightOnlyDF_shader, "mask")
    End Sub
    '==============================================================================================================
    '==============================================================================================================
    'SolidAlphaTextured
    Public SphereicAlpha_color, SphereicAlpha_camPos, SphereicAlpha_matrix As Integer
    Private Sub set_SphereicAlpha_variables()
        SphereicAlpha_color = glGetUniformLocation(shader_list.SphereicAlpha_shader, "colorMap")
        SphereicAlpha_camPos = glGetUniformLocation(shader_list.SphereicAlpha_shader, "ViewPosition")
        SphereicAlpha_matrix = glGetUniformLocation(shader_list.SphereicAlpha_shader, "ModelViewMatrix")
    End Sub
    '==============================================================================================================
    '==============================================================================================================
    'SolidAlphaTextured
    Public SolidAlphaTextured_color, SolidAlphaTextured_camPos, SolidAlphaTextured_matrix As Integer
    Private Sub set_SolidAlphaTextured_variables()
        SolidAlphaTextured_color = glGetUniformLocation(shader_list.SolidAlphaTextured_shader, "colorMap")
        SolidAlphaTextured_camPos = glGetUniformLocation(shader_list.SolidAlphaTextured_shader, "ViewPosition")
        SolidAlphaTextured_matrix = glGetUniformLocation(shader_list.SolidAlphaTextured_shader, "ModelViewMatrix")
    End Sub
    '==============================================================================================================
    '==============================================================================================================
    'BBbyPointAlpha
    Public BBbyPointAlpha_texture, BBbyPointAlpha_size, BBbyPointAlpha_rotation As Integer
    Private Sub set_BBbyPointAlpha_variables()
        BBbyPointAlpha_texture = glGetUniformLocation(shader_list.BBbyPointAlpha_shader, "colorMap")
        BBbyPointAlpha_size = glGetUniformLocation(shader_list.BBbyPointAlpha_shader, "size")
        BBbyPointAlpha_rotation = glGetUniformLocation(shader_list.BBbyPointAlpha_shader, "rotation")
    End Sub
    '==============================================================================================================
    '==============================================================================================================
    'passthrough
    Public passthrough_color As Integer
    Private Sub set_passthrough_variables()
        passthrough_color = glGetUniformLocation(shader_list.passthrough_shader, "colorMap")
    End Sub
    '==============================================================================================================
    '==============================================================================================================
    Public FXAA_color, FXAA_screenSize As Integer
    Public Sub set_FXAA_variables()
        FXAA_color = glGetUniformLocation(shader_list.FXAA_shader, "colorMap")
        FXAA_screenSize = glGetUniformLocation(shader_list.FXAA_shader, "viewportSize")
    End Sub
    '==============================================================================================================
    Public Sub set_shader_variables()
        set_genericBump_variables()
        set_lightingFBO_variables()
        set_BBbyPointAlpha_variables()
        set_BillBoardBasic_Variables()
        set_BillBoardBasicAlpha_Variables()
        set_LightOnlyDF_variables()
        set_SolidAlphaTextured_variables()
        set_passthrough_variables()
        set_FXAA_variables()
        Return
    End Sub
    '==============================================================================================================

End Module
