Imports System.Math
Imports Tao.OpenGl.Gl
Module mod_Matrix
    Public Function transform(ByVal v As vec3, ByVal m() As Single) As vec3
        Dim vo As vec3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)

        Return vo

    End Function
    Public Function transformGL(ByVal v As vec3, ByVal m() As Single) As vec3
        Dim vo As vec3

        Dim mm(16) As Single
        mm(0) = 1.0 : mm(1) = 0.0 : mm(2) = 0.0 : mm(3) = 0.0
        mm(4) = 0.0 : mm(5) = 1.0 : mm(6) = 0.0 : mm(7) = 0.0
        mm(8) = 0.0 : mm(9) = 0.0 : mm(10) = 1.0 : mm(11) = 0.0
        mm(12) = v.x : mm(13) = v.y : mm(14) = v.z : mm(15) = 1.0

        glPushMatrix()
        'glLoadIdentity()
        glLoadMatrixf(m)
        'glRotatef(Cam_X_angle, 0.0, 1.0, 0.0)
        'glRotatef(Cam_Y_angle, 1.0, 0.0, 0.0)
        glMultMatrixf(mm)
        glGetFloatv(GL_MODELVIEW_MATRIX, mm)
        glPopMatrix()

        vo.x = mm(12)
        vo.y = mm(13)
        vo.z = mm(14)

        Return vo

    End Function

End Module
