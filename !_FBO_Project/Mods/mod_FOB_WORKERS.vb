Imports System.IO
Imports System.Math
Imports System
Imports Tao.OpenGl
Imports Tao.FreeGlut

Module modFBO_WORKERS
    Public FBO_WORKER As Integer
    Public FBO_HUD As New FBO_HUD_
    Public FBO_ALPHA_class As New FBO_ALPHA_class_
    Public FBO_ALPHA As Integer
    Public gColor_HUD1, gColor_HUD2 As Integer
    Public gColor_ALPHA As Integer

    Public Class FBO_HUD_
        Public hud_width As Integer = 800
        Public hud_height As Integer = 600
        Private hud_depth_texture As Integer
        Private attachments_c() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}

        Public Sub shut_down()
            delete_textures_and_fob_objects()
        End Sub
        Private Sub delete_textures_and_fob_objects()
            Dim e As Integer
            If gColor_HUD1 > 0 Then
                Gl.glDeleteTextures(1, gColor_HUD1)
                e = Gl.glGetError
            End If
            If gColor_HUD2 > 0 Then
                Gl.glDeleteTextures(1, gColor_HUD2)
                e = Gl.glGetError
            End If
            If hud_depth_texture > 0 Then
                Gl.glDeleteRenderbuffersEXT(1, hud_depth_texture)
                e = Gl.glGetError
            End If
            If gBufferFBO > 0 Then
                Gl.glDeleteFramebuffersEXT(1, FBO_WORKER)
                e = Gl.glGetError
            End If
        End Sub
        Public Sub getsize(ByRef w As Integer, ByRef h As Integer)
            Dim w1, h1 As Integer
            w1 = frmMain.PB1.Width
            h1 = frmMain.PB1.Height
            w = w1 + (w1 Mod 2)
            h = h1 + (h1 Mod 2)
        End Sub
        Private Sub create_textures()

            ' - gColor_HUD1
            Gl.glGenTextures(1, gColor_HUD1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor_HUD1)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, hud_width, hud_height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Dim e2 = Gl.glGetError
            ' - gColor_HUD2
            Gl.glGenTextures(1, gColor_HUD2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor_HUD2)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, hud_width, hud_height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Dim e3 = Gl.glGetError

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        End Sub
        Public Sub attach_Color_HUD1()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor_HUD1, 0)
            Gl.glDrawBuffers(1, attachments_c)
            Dim er = Gl.glGetError
        End Sub
        Public Sub attach_Color_HUD2()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor_HUD1, 0)
            Gl.glDrawBuffers(1, attachments_c)
            Dim er = Gl.glGetError
        End Sub
        Public Sub detachFBOtextures()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)
        End Sub
        Public Function init() As Boolean

            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)
            Dim e1 = Gl.glGetError

            delete_textures_and_fob_objects()
            'Create the gBuffer textures
            create_textures()
            Dim e2 = Gl.glGetError

            'Create the FBO
            Gl.glGenFramebuffersEXT(1, FBO_WORKER)
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, FBO_WORKER)
            Dim e3 = Gl.glGetError

            Gl.glGenRenderbuffersEXT(1, hud_depth_texture)
            Gl.glBindRenderbufferEXT(Gl.GL_RENDERBUFFER_EXT, hud_depth_texture)
            Gl.glRenderbufferStorageEXT(Gl.GL_RENDERBUFFER_EXT, Gl.GL_DEPTH_COMPONENT16, hud_width, hud_height)
            Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, hud_depth_texture)
            Dim e4 = Gl.glGetError


            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor_HUD1, 0)
            Dim e5 = Gl.glGetError

            'attach draw buffers
            Gl.glDrawBuffers(1, attachments_c)

            'attach draw buffers
            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
                Return False
            End If


            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)


            Return True
        End Function
    End Class

    Public Class FBO_ALPHA_class_
        Public alpha_width As Integer = 800
        Public alpha_height As Integer = 600
        Private alpha_depth_texture As Integer
        Private attachments_c() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}

        Public Sub shut_down()
            delete_textures_and_fob_objects()
        End Sub
        Private Sub delete_textures_and_fob_objects()
            Dim e As Integer
            If gColor_ALPHA > 0 Then
                Gl.glDeleteTextures(1, gColor_ALPHA)
                e = Gl.glGetError
            End If
            If alpha_depth_texture > 0 Then
                Gl.glDeleteRenderbuffersEXT(1, alpha_depth_texture)
                e = Gl.glGetError
            End If
            If FBO_ALPHA > 0 Then
                Gl.glDeleteFramebuffersEXT(1, FBO_ALPHA)
                e = Gl.glGetError
            End If
        End Sub
        Private Sub create_textures()

            ' - gColor_ALPHA
            Gl.glGenTextures(1, gColor_ALPHA)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor_ALPHA)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, alpha_width, alpha_height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Dim e2 = Gl.glGetError

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        End Sub
        Public Sub attach_Color_alpha()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor_ALPHA, 0)
            Gl.glDrawBuffers(1, attachments_c)
            Dim er = Gl.glGetError
        End Sub
        Public Sub attach_Color_HUD2()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor_HUD1, 0)
            Gl.glDrawBuffers(1, attachments_c)
            Dim er = Gl.glGetError
        End Sub
        Public Sub detachFBOtextures()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)
        End Sub
        Public Function init() As Boolean

            G_Buffer.getsize(alpha_width, alpha_height)
            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)
            Dim e1 = Gl.glGetError

            delete_textures_and_fob_objects()
            'Create the gBuffer textures
            create_textures()
            Dim e2 = Gl.glGetError

            'Create the FBO
            Gl.glGenFramebuffersEXT(1, FBO_ALPHA)
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, FBO_ALPHA)
            Dim e3 = Gl.glGetError

            'Gl.glGenRenderbuffersEXT(1, alpha_depth_texture)
            Gl.glBindRenderbufferEXT(Gl.GL_RENDERBUFFER_EXT, grDepth)
            Gl.glRenderbufferStorageEXT(Gl.GL_RENDERBUFFER_EXT, Gl.GL_DEPTH_COMPONENT24, alpha_width, alpha_height)
            Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, grDepth)
            Dim e4 = Gl.glGetError



            'attach draw buffers
            attach_Color_alpha()
            Dim e5 = Gl.glGetError
            'attach draw buffers
            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
                Return False
            End If


            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)

            Return True
        End Function
    End Class
    '============================================================================================================
    Public FBO_Final As New FBO_Final_
    Public TEMP_FBO As Integer
    Public FINAL_COLOR As Integer
    Public Class FBO_Final_
        Dim depth As Integer
        Dim attachment() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}
        Dim xs, ys As Integer
        Public Sub shut_down()
            delete_FBO()
        End Sub
        Public Sub delete_FBO()
            If FINAL_COLOR > 0 Then
                Gl.glDeleteTextures(1, FINAL_COLOR)
            End If
            If depth > 0 Then
                Gl.glDeleteRenderbuffersEXT(1, depth)
            End If
            If TEMP_FBO > 0 Then
                Gl.glDeleteFramebuffersEXT(1, TEMP_FBO)
            End If

        End Sub
        Private Sub make_textures()

            ' - color
            Gl.glGenTextures(1, FINAL_COLOR)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, FINAL_COLOR)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, xs, ys, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)



        End Sub
        Public Sub init()
            G_Buffer.getsize(xs, ys)
            delete_FBO()
            make_textures()

            Gl.glGenFramebuffersEXT(1, TEMP_FBO)
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, TEMP_FBO)
            Dim e3 = Gl.glGetError

            Gl.glGenRenderbuffersEXT(1, depth)
            Gl.glBindRenderbufferEXT(Gl.GL_RENDERBUFFER_EXT, depth)
            Gl.glRenderbufferStorageEXT(Gl.GL_RENDERBUFFER_EXT, Gl.GL_DEPTH_COMPONENT16, xs, ys)
            Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, depth)
            Dim e4 = Gl.glGetError


            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, FINAL_COLOR, 0)
            Dim e5 = Gl.glGetError

            'attach draw buffers
            Gl.glDrawBuffers(1, attachment)

            'attach draw buffers
            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
            End If
        End Sub

    End Class
End Module
