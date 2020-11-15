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


Module mod_OpenGL
    Public position0() As Single = {3.535534F, 2.5F, -3.535534F, 1.0F}
    Public position1() As Single = {5.0F, 8.0F, -5.0F, 1.0F}
    Public position2() As Single = {0.0F, 10.0F, 0.0F, 1.0F}
    '==============================
    Public largestAnsio As Single
    Public grid As Integer
    '==============================
    Public pb1_hDC As System.IntPtr
    Public pb1_hRC As System.IntPtr
    Public pb2_hDC As System.IntPtr
    Public pb2_hRC As System.IntPtr
    Public pb3_hDC As System.IntPtr
    Public pb3_hRC As System.IntPtr

    Public Sub EnableOpenGL()
        frmMain.pb2.Visible = False
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        pb1_hDC = User.GetDC(frmMain.pb1.Handle)
        pb2_hDC = User.GetDC(frmMain.pb2.Handle)
        pb3_hDC = User.GetDC(frmMain.PB3.Handle)
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        frmMain.pb2.Location = frmMain.pb1.Location
        Dim pfd As Gdi.PIXELFORMATDESCRIPTOR
        Dim PixelFormat As Integer

        'ZeroMemory(pfd, Len(pfd))
        pfd.nSize = Len(pfd)
        pfd.nVersion = 1
        pfd.dwFlags = Gdi.PFD_DRAW_TO_WINDOW Or Gdi.PFD_SUPPORT_OPENGL Or Gdi.PFD_DOUBLEBUFFER Or Gdi.PFD_GENERIC_ACCELERATED
        pfd.iPixelType = Gdi.PFD_TYPE_RGBA
        pfd.cColorBits = 32
        pfd.cDepthBits = 32
        pfd.cStencilBits = 8
        pfd.cAlphaBits = 8
        pfd.iLayerType = Gdi.PFD_MAIN_PLANE

        PixelFormat = Gdi.ChoosePixelFormat(pb1_hDC, pfd)
        PixelFormat = Gdi.ChoosePixelFormat(pb2_hDC, pfd)
        PixelFormat = Gdi.ChoosePixelFormat(pb3_hDC, pfd)
        If PixelFormat = 0 Then
            MessageBox.Show("Unable to retrieve pixel format")
            End
        End If
        '================================================================1
        If Not (Gdi.SetPixelFormat(pb1_hDC, PixelFormat, pfd)) Then
            MessageBox.Show("Unable to set pixel format")
            End
        End If
        pb1_hRC = Wgl.wglCreateContext(pb1_hDC)
        If pb1_hRC.ToInt32 = 0 Then
            MessageBox.Show("Unable to get rendering context")
            End
        End If
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 1")
            End
        End If
        '================================================================2
        If Not (Gdi.SetPixelFormat(pb2_hDC, PixelFormat, pfd)) Then
            MessageBox.Show("Unable to set pixel format 2")
            End
        End If
        pb2_hRC = Wgl.wglCreateContext(pb2_hDC)
        If pb2_hRC.ToInt32 = 0 Then
            MessageBox.Show("Unable to get rendering context 2")
            End
        End If
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 2")
            End
        End If
        '================================================================3
        If Not (Gdi.SetPixelFormat(pb3_hDC, PixelFormat, pfd)) Then
            MessageBox.Show("Unable to set pixel format 3")
            End
        End If
        pb3_hRC = Wgl.wglCreateContext(pb3_hDC)
        If pb3_hRC.ToInt32 = 0 Then
            MessageBox.Show("Unable to get rendering context 3")
            End
        End If
        If Not (Wgl.wglMakeCurrent(pb3_hDC, pb3_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 3 ")
            End
        End If
        '================================================================
        'go back to context 1
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 1")
            End
        End If

        Glut.glutInit() 'fire up glut
        'get max ansio level
        glGetFloatv(GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)

        Gl.glClampColorARB(Gl.GL_CLAMP_VERTEX_COLOR_ARB, Gl.GL_FALSE)

        'Glut.glutInitDisplayMode(GLUT_RGBA Or GLUT_DOUBLE Or GLUT_MULTISAMPLE)
        Glut.glutInitDisplayMode(GLUT_RGBA Or GLUT_SINGLE)
        frmMain.PB1.Dock = DockStyle.Fill
        Application.DoEvents()

        frmMain.PB2.Size = frmMain.PB1.Size
        frmMain.PB3.Size = frmMain.PB3.Size

        frmMain.PB2.Visible = False
        frmMain.PB3.Visible = False
        glViewport(0, 0, frmMain.pb1.Width, frmMain.pb1.Height)

        glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        glEnable(gL_COLOR_MATERIAL)
        glEnable(gL_LIGHT0)
        glEnable(gL_LIGHTING)
        Wgl.wglShareLists(pb1_hRC, pb2_hRC)
        Wgl.wglShareLists(pb1_hRC, pb3_hRC)
        'FBO_HUD.init()
        gl_set_lights()
        Dim pa = Wgl.wglGetProcAddress("wglGetExtensionsStringEXT")

    End Sub

    Public Sub DisableOpenGL()
        G_Buffer.shut_down()
        FBO_ALPHA_class.shut_down()
        FBO_Final.shut_down()
        Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero)
        Wgl.wglDeleteContext(pb1_hRC)

    End Sub
    Public Sub glutPrint(ByVal x As Single, ByVal y As Single, _
ByVal text As String, ByVal r As Single, ByVal g As Single, ByVal b As Single, ByVal a As Single)

        Try
            If text.Length = 0 Then Exit Sub
        Catch ex As Exception
            Return
        End Try
        Dim blending As Boolean = False
        If glIsEnabled(gL_BLEND) Then blending = True
        glEnable(gL_BLEND)
        glColor3f(r, g, b)
        glRasterPos2f(x, y)
        For Each I In text

            Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_8_BY_13, Asc(I))

        Next
        If Not blending Then glDisable(gL_BLEND)
    End Sub
    Public Sub glutPrintBox(ByVal x As Single, ByVal y As Single, _
ByVal text As String, ByVal r As Single, ByVal g As Single, ByVal b As Single, ByVal a As Single)

        Try
            If text.Length = 0 Then Exit Sub
        Catch ex As Exception
            Return
        End Try
        Dim blending As Boolean = False
        glPolygonMode(gL_FRONT_AND_BACK, gL_FILL)
        If glIsEnabled(gL_BLEND) Then blending = True
        glEnable(gL_BLEND)
        glColor4f(0, 0, 0, 0.5)
        glBegin(gL_QUADS)
        Dim L1 = text.Length * 8
        Dim l2 = 7
        glVertex2f(x - 2, y - l2 + 2)
        glVertex2f(x + L1 + 2, y - l2 + 2)
        glVertex2f(x + L1 + 2, y + l2 + 5)
        glVertex2f(x - 2, y + l2 + 5)
        glEnd()
        glColor3f(r, g, b)
        glRasterPos2f(x, y)
        For Each I In text

            Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_8_BY_13, Asc(I))

        Next
        If Not blending Then glDisable(gL_BLEND)
    End Sub
    Public Sub ResizeGL(ByRef w As Integer, ByRef h As Integer)
        glViewport(0, 0, w, h)
    End Sub

    Public Sub ViewOrtho()
        glMatrixMode(gL_PROJECTION) 'Select Projection
        glLoadIdentity() 'Reset The Matrix
        glOrtho(0, frmMain.pb1.Width, -frmMain.pb1.Height, 0, -200.0, 100.0) 'Select Ortho Mode
        glMatrixMode(gL_MODELVIEW)    'Select Modelview Matrix
        glLoadIdentity() 'Reset The Matrix
    End Sub
    Public Sub ViewPerspective(w, h)
        ' Set Up A Perspective View

        glMatrixMode(gL_PROJECTION) 'Select Projection
        glLoadIdentity()

        Glu.gluPerspective(60.0F, CSng(w / h), 0.1F, far_Clip)
        glEnable(gL_DEPTH_TEST)
        glDepthMask(gL_TRUE)
        glDepthRange(0.0, 1.0)
        glMatrixMode(gL_MODELVIEW)    'Select Modelview
        glLoadIdentity() 'Reset The Matrix
    End Sub

    Public Sub gl_set_lights()
        'lighting

        'Debug.WriteLine("GL Error A:" + glGetError().ToString)
        ''glEnable(gL_SMOOTH)
        ''glShadeModel(gL_SMOOTH)
        'Debug.WriteLine("GL Error B:" + glGetError().ToString)
        Dim global_ambient() As Single = {0.2F, 0.2F, 0.2F, 1.0F}

        Dim specular0() As Single = {0.5F, 0.5F, 0.5F, 1.0F}
        Dim emission0() As Single = {0.0F, 0.0F, 0.0F, 1.0F}
        Dim ambient0() As Single = {0.3F, 0.3F, 0.3F, 1.0F}
        Dim diffuseLight0() As Single = {0.5, 0.5, 0.5, 1.0F}

        Dim specular1() As Single = {0.5F, 0.5F, 0.5F, 1.0F}
        Dim emission1() As Single = {0.0F, 0.0F, 0.0F, 1.0F}
        Dim ambient1() As Single = {0.3F, 0.3F, 0.3F, 1.0F}
        Dim diffuseLight1() As Single = {0.5, 0.5, 0.5, 1.0F}

        Dim specular2() As Single = {0.5F, 0.5F, 0.5F, 1.0F}
        Dim emission2() As Single = {0.0F, 0.0F, 0.0F, 1.0F}
        Dim ambient2() As Single = {0.3F, 0.3F, 0.3F, 1.0F}
        Dim diffuseLight2() As Single = {0.5, 0.5, 0.5, 1.0F}

        Dim specReflection0() As Single = {0.6F, 0.6F, 0.6F, 1.0F}
        Dim specReflection1() As Single = {0.6F, 0.6F, 0.6F, 1.0F}
        Dim specReflection2() As Single = {0.6F, 0.6F, 0.6F, 1.0F}

        Dim mcolor() As Single = {0.2F, 0.2F, 0.2F, 1.0F}
        'glEnable(gL_SMOOTH)
        glShadeModel(gL_SMOOTH)

        glEnable(gL_LIGHT0)
        glEnable(gL_LIGHT1)
        glEnable(gL_LIGHT2)
        glEnable(gL_LIGHTING)

        'light 0
        glLightModelfv(gL_LIGHT_MODEL_AMBIENT, global_ambient)

        position0 = {0.0F, 40.0F, 0.0F, 1.0F}
        position1 = {-40.0F, 20.0F, 40.0F, 1.0F}
        position2 = {40.0F, -20.0F, -40.0F, 1.0F}
        'Dim position3() As Single = {-400.0F, 100.0F, -400.0F, 1.0F}
        'Dim position4() As Single = {-400.0F, 100.0F, 400.0F, 1.0F}

        ' light 1

        glLightfv(gL_LIGHT0, gL_POSITION, position0)
        glLightfv(gL_LIGHT0, gL_SPECULAR, specular0)
        glLightfv(gL_LIGHT0, gL_EMISSION, emission0)
        glLightfv(gL_LIGHT0, gL_DIFFUSE, diffuseLight0)
        glLightfv(gL_LIGHT0, gL_AMBIENT, ambient0)

        glLightfv(gL_LIGHT1, gL_POSITION, position1)
        glLightfv(gL_LIGHT1, gL_SPECULAR, specular1)
        glLightfv(gL_LIGHT1, gL_EMISSION, emission1)
        glLightfv(gL_LIGHT1, gL_DIFFUSE, diffuseLight1)
        glLightfv(gL_LIGHT1, gL_AMBIENT, ambient1)

        glLightfv(gL_LIGHT2, gL_POSITION, position2)
        glLightfv(gL_LIGHT2, gL_SPECULAR, specular2)
        glLightfv(gL_LIGHT2, gL_EMISSION, emission2)
        glLightfv(gL_LIGHT2, gL_DIFFUSE, diffuseLight2)
        glLightfv(gL_LIGHT2, gL_AMBIENT, ambient2)


        glLightModelfv(gL_LIGHT_MODEL_AMBIENT, global_ambient)


        glPolygonMode(gL_FRONT, gL_FILL)

        glMaterialfv(gL_FRONT, gL_AMBIENT_AND_DIFFUSE, mcolor)
        glMaterialfv(gL_FRONT, gL_SPECULAR, specReflection0)
        glMaterialfv(gL_FRONT, gL_DIFFUSE, diffuseLight0)
        glColorMaterial(gL_FRONT, gL_SPECULAR Or gL_AMBIENT_AND_DIFFUSE)


        glMateriali(gL_FRONT, gL_SHININESS, 100)
        glHint(gL_PERSPECTIVE_CORRECTION_HINT, gL_NICEST)
        glEnable(gL_COLOR_MATERIAL)
        glLightModeli(gL_LIGHT_MODEL_TWO_SIDE, gL_FALSE)

        'glFrontFace(gL_CCW)
        glClearDepth(1.0F)
        glEnable(gL_DEPTH_TEST)
        glLightModelfv(gL_LIGHT_MODEL_LOCAL_VIEWER, 0.0F)
        glEnable(gL_NORMALIZE)


    End Sub
    Public Sub make_xy_grid()
        grid = glGenLists(1)
        glNewList(grid, gL_COMPILE)
        draw_XZ_grid()
        glEndList()
    End Sub
    Public Sub draw_XZ_grid()
        glDisable(gL_LIGHTING)
        glLineWidth(1)
        glBegin(gL_LINES)
        glColor3f(0.3F, 0.3F, 0.3F)
        For z As Single = -100.0F To -1.0F Step 1.0
            glNormal3f(0.0!, 1.0!, 0.0!)
            glVertex3f(-100.0F, 0.0F, z)
            glNormal3f(0.0!, 1.0!, 0.0!)
            glVertex3f(100.0F, 0.0F, z)
        Next
        For z As Single = 1.0F To 100.0F Step 1.0
            glNormal3f(0.0!, 1.0!, 0.0!)
            glVertex3f(-100.0F, 0.0F, z)
            glNormal3f(0.0!, 1.0!, 0.0!)
            glVertex3f(100.0F, 0.0F, z)
        Next
        For x As Single = -100.0F To -1.0F Step 1.0
            glNormal3f(0.0!, 1.0!, 0.0!)
            glVertex3f(x, 0.0F, 100.0F)
            glNormal3f(0.0!, 1.0!, 0.0!)
            glVertex3f(x, 0.0F, -100.0F)
        Next
        For x As Single = 1.0F To 100.0F Step 1.0
            glNormal3f(0.0!, 1.0!, 0.0!)
            glVertex3f(x, 0.0F, 100.0F)
            glNormal3f(0.0!, 1.0!, 0.0!)
            glVertex3f(x, 0.0F, -100.0F)
        Next
        glEnd()
        glLineWidth(1)
        glBegin(gL_LINES)
        glColor3f(0.6F, 0.6F, 0.6F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(1.0F, 0.0F, 0.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(-1.0F, 0.0F, 0.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(0.0F, 0.0F, 1.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(0.0F, 0.0F, -1.0F)
        glEnd()
        'begin axis markers
        ' red is z+
        ' green is x-
        'blue is z-
        ' yellow x+
        glLineWidth(1)

        glBegin(gL_LINES)
        'z+ red
        glColor3f(1.0F, 0.0F, 0.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(0.0F, 0.0F, 1.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(0.0F, 0.0F, 100.0F)
        'z- blue
        glColor3f(0.0F, 0.0F, 1.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(0.0F, 0.0F, -1.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(0.0F, 0.0F, -100.0F)
        'x+ yellow
        glColor3f(1.0F, 1.0F, 0.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(1.0F, 0.0F, 0.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(100.0F, 0.0F, 0.0F)
        'x- green
        glColor3f(0.0F, 1.0F, 0.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(-1.0F, 0.0F, 0.0F)
        glNormal3f(0.0!, 1.0!, 0.0!)
        glVertex3f(-100.0F, 0.0F, 0.0F)
        '---------
        glEnd()

        glEnable(gL_LIGHTING)

    End Sub

End Module
