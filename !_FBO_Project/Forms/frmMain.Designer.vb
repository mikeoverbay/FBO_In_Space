<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.MM = New System.Windows.Forms.MenuStrip()
        Me.m_tools = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_edit_shaders = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_buffers = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_lights = New System.Windows.Forms.ToolStripMenuItem()
        Me.PB1 = New System.Windows.Forms.Panel()
        Me.PB3 = New System.Windows.Forms.Panel()
        Me.PB2 = New System.Windows.Forms.Panel()
        Me.Startup_Timer = New System.Windows.Forms.Timer(Me.components)
        Me.m_wire_frame = New System.Windows.Forms.ToolStripMenuItem()
        Me.MM.SuspendLayout()
        Me.SuspendLayout()
        '
        'MM
        '
        Me.MM.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tools})
        Me.MM.Location = New System.Drawing.Point(0, 0)
        Me.MM.Name = "MM"
        Me.MM.Size = New System.Drawing.Size(474, 24)
        Me.MM.TabIndex = 0
        Me.MM.Text = "MenuStrip1"
        '
        'm_tools
        '
        Me.m_tools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_edit_shaders, Me.m_show_buffers, Me.m_show_lights, Me.m_wire_frame})
        Me.m_tools.Name = "m_tools"
        Me.m_tools.Size = New System.Drawing.Size(47, 20)
        Me.m_tools.Text = "Tools"
        '
        'm_edit_shaders
        '
        Me.m_edit_shaders.Name = "m_edit_shaders"
        Me.m_edit_shaders.Size = New System.Drawing.Size(152, 22)
        Me.m_edit_shaders.Text = "Edit Shaders"
        '
        'm_show_buffers
        '
        Me.m_show_buffers.CheckOnClick = True
        Me.m_show_buffers.Name = "m_show_buffers"
        Me.m_show_buffers.Size = New System.Drawing.Size(152, 22)
        Me.m_show_buffers.Text = "Show Buffers"
        '
        'm_show_lights
        '
        Me.m_show_lights.CheckOnClick = True
        Me.m_show_lights.Name = "m_show_lights"
        Me.m_show_lights.Size = New System.Drawing.Size(152, 22)
        Me.m_show_lights.Text = "Show Lights"
        '
        'PB1
        '
        Me.PB1.Location = New System.Drawing.Point(12, 48)
        Me.PB1.Name = "PB1"
        Me.PB1.Size = New System.Drawing.Size(200, 100)
        Me.PB1.TabIndex = 1
        '
        'PB3
        '
        Me.PB3.Location = New System.Drawing.Point(229, 48)
        Me.PB3.Name = "PB3"
        Me.PB3.Size = New System.Drawing.Size(200, 100)
        Me.PB3.TabIndex = 2
        '
        'PB2
        '
        Me.PB2.Location = New System.Drawing.Point(12, 256)
        Me.PB2.Name = "PB2"
        Me.PB2.Size = New System.Drawing.Size(200, 100)
        Me.PB2.TabIndex = 2
        '
        'Startup_Timer
        '
        Me.Startup_Timer.Interval = 500
        '
        'm_wire_frame
        '
        Me.m_wire_frame.CheckOnClick = True
        Me.m_wire_frame.Name = "m_wire_frame"
        Me.m_wire_frame.Size = New System.Drawing.Size(152, 22)
        Me.m_wire_frame.Text = "Wire Frame"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(474, 368)
        Me.Controls.Add(Me.PB3)
        Me.Controls.Add(Me.PB2)
        Me.Controls.Add(Me.PB1)
        Me.Controls.Add(Me.MM)
        Me.MainMenuStrip = Me.MM
        Me.Name = "frmMain"
        Me.Text = "OpenGL and BEYOND!"
        Me.MM.ResumeLayout(False)
        Me.MM.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MM As System.Windows.Forms.MenuStrip
    Friend WithEvents PB1 As System.Windows.Forms.Panel
    Friend WithEvents PB3 As System.Windows.Forms.Panel
    Friend WithEvents PB2 As System.Windows.Forms.Panel
    Friend WithEvents Startup_Timer As System.Windows.Forms.Timer
    Friend WithEvents m_tools As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_edit_shaders As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_show_buffers As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_show_lights As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_wire_frame As System.Windows.Forms.ToolStripMenuItem

End Class
