Module modLightingGlobals
    Public max_space_lights As Integer = 1024
    Public max_instrument_lights As Integer = 32
    Public max_shiphull_lights As Integer = 32

    'space lights
    Public spaceL_color(max_space_lights * 4) As Single ' color xyz and w as specular level
    Public spaceL_position(max_space_lights * 4) As Single ' pos rgb and w as range
    Public RENDER_SPACE_LIGHTS As Boolean ' turn them on and off for debug

    'instrument
    Public insturmentL_color(max_instrument_lights * 4) As Single ' color xyz and w as specular level
    Public instrumentL_position(max_instrument_lights * 4) As Single ' pos rgb and w as range
    Public RENDER_INSTURMENT_LIGHTS As Boolean ' turn them on and off for debug
    Public INSTURMENT_LIGHT_COUNT As Integer

    'ship hull lighting
    Public shiphullL_color(max_shiphull_lights * 4) As Single ' color xyz and w as specular level
    Public shiphullL_position(max_shiphull_lights * 4) As Single ' pos rgb and w as range
    Public RENDER_SHIPHULL_LIGHTS As Boolean  ' turn them on and off for debug
    Public SHIPHULL_LIGHT_COUNT As Integer

    Public cr() As Single = {1.0, 0.0, 0.0}
    Public cg() As Single = {0.0, 1.0, 0.0}
    Public cb() As Single = {0.0, 0.0, 1.0}
    Public cy() As Single = {1.0, 1.0, 0.0}
    Public cw() As Single = {1.0, 1.0, 1.0}







End Module
