Imports System.IO
Imports System.Math
Imports System
Imports Tao.OpenGl.Gl
Imports Tao.FreeGlut

Module modFBO
    Public G_Buffer As New GBuffer_
    Public gBufferFBO As Integer
    Public gColor, gDepth, gPosition, gNormal, gTEMP As Integer
    Public grDepth As Integer
    Public rendered_shadow_texture As Integer
    Public Class GBuffer_
        Public fbo_w As Integer = -1
        Public fbo_h As Integer = -1
        Private attachments_c() As Integer = {GL_COLOR_ATTACHMENT0_EXT}
        Private attachments_cnp() As Integer = {GL_COLOR_ATTACHMENT0_EXT, GL_COLOR_ATTACHMENT1_EXT, GL_COLOR_ATTACHMENT2_EXT}
        Private attachments_cn() As Integer = {GL_COLOR_ATTACHMENT0_EXT, GL_COLOR_ATTACHMENT1_EXT}
        Private attachments_np() As Integer = {GL_COLOR_ATTACHMENT1_EXT, GL_COLOR_ATTACHMENT2_EXT}

        Public Sub shut_down()
            delete_textures_and_fob_objects()
            'blm_fbo.shutdown_blm_fbo()
        End Sub
        Private Sub delete_textures_and_fob_objects()
            Dim e As Integer
            If gPosition > 0 Then
                glDeleteTextures(1, gPosition)
                e = glGetError
            End If
            If gColor > 0 Then
                glDeleteTextures(1, gColor)
                e = glGetError()
            End If
            If gTEMP > 0 Then
                glDeleteTextures(1, gTEMP)
                e = glGetError()
            End If
            If gDepth > 0 Then
                glDeleteTextures(1, gDepth)
                e = glGetError()
            End If
            If gNormal > 0 Then
                glDeleteTextures(1, gNormal)
                e = glGetError
            End If
            If grDepth > 0 Then
                glDeleteRenderbuffersEXT(1, grDepth)
                e = glGetError
            End If
            If gBufferFBO > 0 Then
                glDeleteFramebuffersEXT(1, gBufferFBO)
                e = glGetError
            End If
        End Sub
        Public Sub getsize(ByRef w As Integer, ByRef h As Integer)
            Dim w1, h1 As Integer
            w1 = frmMain.PB1.Width
            h1 = frmMain.PB1.Height
            w = w1 '+ (w1 Mod 2)
            h = h1 '+ (h1 Mod 2)
            fbo_w = w
            fbo_h = h
        End Sub
        Private Sub create_textures()
            Dim SCR_WIDTH, SCR_HEIGHT As Integer
            getsize(SCR_WIDTH, SCR_HEIGHT)
            'depth buffer
            Dim e1 = glGetError

            ' - Normal buffer
            glGenTextures(1, gNormal)
            glBindTexture(GL_TEXTURE_2D, gNormal)
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA16F_ARB, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGBA, GL_FLOAT, Nothing)
            glTexParameteri(GL_TEXTURE_2D, GL_GENERATE_MIPMAP, GL_FALSE)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST)
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE)
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE)
            ' - depth buffer
            glGenTextures(1, gDepth)
            glBindTexture(GL_TEXTURE_2D, gDepth)
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA16F_ARB, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGBA, GL_FLOAT, Nothing)
            glTexParameteri(GL_TEXTURE_2D, GL_GENERATE_MIPMAP, GL_FALSE)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST)
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE)
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE)
            Dim e5 = glGetError
            ' - rendered shadow texture
            glGenTextures(1, gPosition)
            glBindTexture(GL_TEXTURE_2D, gPosition)
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB16F_ARB, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, Nothing)
            glTexParameteri(GL_TEXTURE_2D, GL_GENERATE_MIPMAP, GL_FALSE)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST)
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE)
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE)
            Dim e0 = glGetError
            ' - Color color buffer
            glGenTextures(1, gColor)
            glBindTexture(GL_TEXTURE_2D, gColor)
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGBA, GL_UNSIGNED_BYTE, Nothing)
            glTexParameteri(GL_TEXTURE_2D, GL_GENERATE_MIPMAP, GL_FALSE)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST)
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE)
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE)
            Dim e2 = glGetError
            ' - Color color buffer
            glGenTextures(1, gTEMP)
            glBindTexture(GL_TEXTURE_2D, gTEMP)
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGBA, GL_UNSIGNED_BYTE, Nothing)
            glTexParameteri(GL_TEXTURE_2D, GL_GENERATE_MIPMAP, GL_FALSE)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST)
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE)
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE)
            Dim e3 = glGetError

            glBindTexture(GL_TEXTURE_2D, 0)

        End Sub

        Public Sub attach_Color_Normal_Position()
            'detachFBOtextures()
            'glBindTexture(GL_TEXTURE_2D, gColor)
            'glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT0_EXT, GL_TEXTURE_2D, gColor, 0)
            'glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT1_EXT, GL_TEXTURE_2D, gNormal, 0)
            'glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT2_EXT, GL_TEXTURE_2D, gPosition, 0)
            glDrawBuffers(3, attachments_cnp)
            Dim er = glGetError
        End Sub
        Public Sub attach_Color_Normal()
            glDrawBuffers(2, attachments_cn)
            Dim er = glGetError
        End Sub
        Public Sub attach_Normal_position()
            glDrawBuffers(2, attachments_np)
            Dim er = glGetError
        End Sub
        Public Sub attachColor_And_blm_tex1()
            ' detachFBOtextures()
            'glBindTexture(GL_TEXTURE_2D, gColor)
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT0_EXT, GL_TEXTURE_2D, gColor, 0)
            'glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT1_EXT, GL_TEXTURE_2D, blm_fbo.blm_tex1, 0)
            glDrawBuffers(2, attachments_cn)
            Dim er = glGetError
        End Sub
        Public Sub attachColor_And_Normal_FOG_Texture()
            Dim attachers3() = {GL_COLOR_ATTACHMENT0_EXT, GL_COLOR_ATTACHMENT1_EXT, GL_COLOR_ATTACHMENT2_EXT}
            detachFBOtextures()
            'glBindTexture(GL_TEXTURE_2D, gColor)
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT0_EXT, GL_TEXTURE_2D, gColor, 0)
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT1_EXT, GL_TEXTURE_2D, gNormal, 0)
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT2_EXT, GL_TEXTURE_2D, gTEMP, 0)
            glDrawBuffers(3, attachers3)
            glBindTexture(GL_TEXTURE_2D, 0)
            'Dim er = glGetError
        End Sub

        Public Sub attachColorTexture()
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT0_EXT, GL_TEXTURE_2D, gColor, 0)
            glDrawBuffers(1, attachments_c)
        End Sub
        Public Sub attach_TEMP()
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT0_EXT, GL_TEXTURE_2D, gTEMP, 0)
            glDrawBuffers(1, attachments_c)
        End Sub
        Public Sub attachFXAAtexture()
            'detachFBOtextures()
            'glBindTexture(GL_TEXTURE_2D, gTEMP)
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT0_EXT, GL_TEXTURE_2D, gTEMP, 0)
            glDrawBuffers(1, attachments_c)
            'glBindTexture(GL_TEXTURE_2D, 0)
            'Dim er = glGetError
        End Sub
        Public Sub detachFBOtextures()
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT0_EXT, GL_TEXTURE_2D, 0, 0)
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT1_EXT, GL_TEXTURE_2D, 0, 0)
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT2_EXT, GL_TEXTURE_2D, 0, 0)
        End Sub
        Public Function init() As Boolean
            If Not _STARTED Then Return False
            Threading.Thread.Sleep(50)
            Dim SCR_WIDTH, SCR_HEIGHT As Integer
            getsize(SCR_WIDTH, SCR_HEIGHT)

            glBindFramebufferEXT(GL_DRAW_FRAMEBUFFER_EXT, 0)
            Dim e1 = glGetError()

            'blm_fbo.reset_blm_fbo() ' reset blm_fbo as its size must match

            delete_textures_and_fob_objects()
            'Create the gBuffer textures
            create_textures()
            Dim e2 = glGetError()

            'Create the FBO
            glGenFramebuffersEXT(1, gBufferFBO)
            glBindFramebufferEXT(GL_FRAMEBUFFER_EXT, gBufferFBO)
            Dim e3 = glGetError()

            glGenRenderbuffersEXT(1, grDepth)
            glBindRenderbufferEXT(GL_RENDERBUFFER_EXT, grDepth)
            glRenderbufferStorageEXT(GL_RENDERBUFFER_EXT, GL_DEPTH_COMPONENT32, SCR_WIDTH, SCR_HEIGHT)
            glFramebufferRenderbufferEXT(GL_FRAMEBUFFER_EXT, GL_DEPTH_ATTACHMENT_EXT, GL_RENDERBUFFER_EXT, grDepth)
            Dim e4 = glGetError()


            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT0_EXT, GL_TEXTURE_2D, gColor, 0)
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT1_EXT, GL_TEXTURE_2D, gNormal, 0)
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT, GL_COLOR_ATTACHMENT2_EXT, GL_TEXTURE_2D, gPosition, 0)
            Dim e5 = glGetError()

            'attach draw buffers
            'glDrawBuffers(3, attachments_cnp)

            'attach draw buffers
            Dim Status = glCheckFramebufferStatusEXT(GL_FRAMEBUFFER_EXT)

            If Status <> GL_FRAMEBUFFER_COMPLETE_EXT Then
                MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
                Return False
            End If


            glBindFramebufferEXT(GL_FRAMEBUFFER_EXT, 0)

            Return True
        End Function

        Public Sub get_depth_buffer(ByVal w As Integer, ByVal h As Integer)
            Dim e1 = glGetError()
            glBindTexture(GL_TEXTURE_2D, gDepth)
            glCopyTexImage2D(GL_TEXTURE_2D, 0, GL_DEPTH_COMPONENT24, 0, 0, w, h, 0)
            Dim e2 = glGetError()
            glBindTexture(GL_TEXTURE_2D, 0)
        End Sub


    End Class


End Module
