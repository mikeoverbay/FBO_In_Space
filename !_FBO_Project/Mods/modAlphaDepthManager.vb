Imports Tao.OpenGl.Gl
Module modAlphaDepthManager

    'this manages the sorting of the alpha objects.
    '=============================
    'object types
    ' 1 light position
    ' 2 emitters
    '
    '=============================
    '===========================================================
    '
    '
    '===========================================================
    Dim st As New Stopwatch
    Public Sub sort_alpha_items()
        st.Restart()
        add_alpha_items()
        Dim cnt = alpha_order_list_count - 1
        Dim t As New sp_pointer_
        Dim v As vec3
        Dim len As Single
        Dim idx As Integer
        Dim ty As Integer
        glPointSize(6)
        For i = 0 To cnt
            ty = alpha_order_list(i).object_type
            idx = alpha_order_list(i).index
            Select Case ty
                Case 1
                    v.x = spaceL_position((idx * 4) + 0)
                    v.y = spaceL_position((idx * 4) + 1)
                    v.z = spaceL_position((idx * 4) + 2)
                Case 2
                    emitters(idx).get_location(v)

            End Select
            v = transform(v, model_view)
            alpha_order_list(i).dist = (v.x - eyeX) + (v.y - eyeY) + (v.z - eyeZ)
        Next
        glPointSize(1)
        Dim sp As Boolean = True
        While sp
            sp = False
            For i = 0 To cnt - 1
                If alpha_order_list(i).dist > alpha_order_list(i + 1).dist Then
                    sp = True
                    idx = alpha_order_list(i).index
                    len = alpha_order_list(i).dist
                    ty = alpha_order_list(i).object_type
                    alpha_order_list(i).index = alpha_order_list(i + 1).index
                    alpha_order_list(i).dist = alpha_order_list(i + 1).dist
                    alpha_order_list(i).object_type = alpha_order_list(i + 1).object_type
                    alpha_order_list(i + 1).index = idx
                    alpha_order_list(i + 1).dist = len
                    alpha_order_list(i + 1).object_type = ty
                End If
            Next
        End While
        Dim et = st.ElapsedMilliseconds

    End Sub
    Public alpha_order_list_count As Integer
    Public Sub build_alpha_object_list()
        'object types
        ' 1 light position
        ' 2 emitters
        '
        alpha_order_list_count = EMITTER_COUNT + SPACE_LIGHT_COUNT
        ReDim alpha_order_list(alpha_order_list_count)
        add_alpha_items()
    End Sub
    Public Sub add_alpha_items()
        alpha_order_list_count = (EMITTER_COUNT + SPACE_LIGHT_COUNT)
        Dim cnt As Integer = 0
        'add space lights
        For i = 0 To SPACE_LIGHT_COUNT - 1
            alpha_order_list(cnt).index = i
            alpha_order_list(cnt).object_type = 1
            cnt += 1
        Next
        'add emitters
        For i = 0 To EMITTER_COUNT - 1
            alpha_order_list(cnt).index = i
            alpha_order_list(cnt).object_type = 2
            cnt += 1
        Next

    End Sub
    Public alpha_order_list(0) As sp_pointer_

    Public Structure sp_pointer_
        Public index As Integer
        Public dist As Single
        Public object_type As Integer
    End Structure

End Module
