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

Module modEffectClasses
    Public emitters(emitter_count) As smoke_
    Public smoke_emitter As New smoke_
    Dim tread_start_count As Integer = 0
    Public not_active As Integer = 0

    '===========================================================
    '
    '
    '===========================================================
    Public Class smoke_
        'e_types
        '0 = explosion
        '1 = continuous smoke
        '3  fading smoke
        '4  good explosion
        Private eble As Boolean
        Public e_type As Integer
        Private size As Single
        Public fade_speed As Single
        Public active As Boolean ' make it false to kill the thread
        Public update_thread As System.Threading.Thread
        Private current_frame As Integer
        Public SF As Integer
        Public location As vec3
        Private scale As Single
        Private rotation As Single
        Private rot_enable As Boolean
        Public Sub start()
            active = True
            Me.update_thread = New Thread(AddressOf update)
            Me.update_thread.Priority = ThreadPriority.Lowest
            Me.update_thread.Name = "Emitter"
            Me.update_thread.IsBackground = True
            Me.update_thread.Start()
        End Sub
        Public Sub set_rotation(ByVal r As Single)
            rotation = r
        End Sub
        Public Sub enable_rotation(ByVal e As Boolean)
            rot_enable = e
        End Sub
        Public Sub enable(ByVal e As Boolean)
            eble = e
        End Sub
        Public Sub set_start_frame(ByVal f As Integer)
            Me.current_frame = f
        End Sub
        Public Sub abort()
            Me.active = False
        End Sub
        Public Sub setScale(ByVal s As Single)
            Me.size = s
        End Sub
        Public Sub set_e_type(ByVal t As Integer)
            e_type = t
        End Sub
        Public Sub get_location(ByRef v As vec3)
            v.x = location.x
            v.y = location.y
            v.z = location.z
        End Sub
        Public Sub update()
            'Me.sw = New Stopwatch
            'Me.sw.Start()
            While active
                current_frame += 1
                If current_frame > 90 Then
                    current_frame = 0
                End If
                scale = CSng(current_frame) / 90.0!
                If rot_enable Then
                    rotation = PI * 2 * scale
                End If
                Thread.Sleep(33)
            End While
        End Sub
        Public Sub draw_particles()
            If Not eble Then Return

            glUniform1f(BBbyPointAlpha_rotation, rotation)
            'glBindTexture(GL_TEXTURE_2D, boobs_texture)
            Gl.glPushMatrix()
            Gl.glTranslatef(Me.location.x, Me.location.y, Me.location.z)
            Select Case e_type
                Case 0
                    glBindTexture(GL_TEXTURE_2D, smoke_testures(current_frame))
                    glUniform1f(BBbyPointAlpha_size, scale * size + 10.0)
                    glColor4f(1.0, scale + 0.4, scale + 0.4, (1.0 - scale) + 0.5)

                Case 1
                    glBindTexture(GL_TEXTURE_2D, smoke_testures(current_frame))
                    glUniform1f(BBbyPointAlpha_size, size)
                    glColor4f(1.0, 1.0, 1.0, 1.0)
                Case 2
                    glBindTexture(GL_TEXTURE_2D, smoke_testures(current_frame))
                    glUniform1f(BBbyPointAlpha_size, size)
                    glColor4f(1.0, 1.0, 1.0, 1.0)
                Case 4
                    glBindTexture(GL_TEXTURE_2D, explosion_1_testures(current_frame))
                    'glBindTexture(GL_TEXTURE_2D, boobs_texture)
                    glUniform1f(BBbyPointAlpha_size, size)
                    glColor4f(1.0, 1.0, 1.0, 1.0)

            End Select

            glBegin(GL_POINTS)
            Gl.glVertex3f(0.0!, 0.0!, 0.0!)
            Gl.glEnd()
            Gl.glPopMatrix()
            'free binding / disable shader
            glBindTexture(GL_TEXTURE_2D, 0)
        End Sub

    End Class
    '===========================================================
    '
    '
    '===========================================================
    Public Structure explostion_
        Const capacity As Integer = 100
        Private current_frame() As Integer
        Private size() As Single
        Private location() As vec3
        Private rotation() As Single
        Public update_thread As System.Threading.Thread

        Public Sub initilize()

            ReDim current_frame(capacity)
            ReDim size(capacity)
            ReDim rotation(capacity)
            ReDim location(capacity)
            For i = 0 To capacity - 1
                location(i) = New vec3
                size(i) = 10.0!
            Next
            Me.update_thread = New Thread(AddressOf updater)

        End Sub
        Private Function check_indx(ByVal i As Integer) As Boolean
            'sanity check that we have a valid indx in to the array sizes
            If i < 0 Or i > capacity - 1 Then
                Return False
            End If
            Return True
        End Function
        Public Function set_size(ByVal indx As Integer, ByVal s As Single) As Boolean
            If Not check_indx(indx) Then
                Return False
            End If
            size(indx) = s
            Return True
        End Function

        Public Function set_location(ByVal indx As Integer, ByRef v As vec3) As Boolean
            If Not check_indx(indx) Then
                Return False
            End If
            location(indx).x = v.x
            location(indx).y = v.y
            location(indx).z = v.z
            Return True
        End Function
        Public Function set_current_frame(ByVal indx As Integer, ByVal f As Integer)
            If Not check_indx(indx) Then
                Return False
            End If
            current_frame(indx) = f
            Return True
        End Function

        Public Function set_rotation(ByVal indx As Integer, ByVal r As Single)
            If Not check_indx(indx) Then
                Return False
            End If
            rotation(indx) = r
            Return True
        End Function

    End Structure

    Private Sub updater()

    End Sub
End Module
