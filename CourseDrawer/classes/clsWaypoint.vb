﻿Public Class clsWaypoint

    Public Shared Event SelectionChanged(ByRef wp As clsWaypoint)
    Private Shared _isAnySelected As Boolean
    Public Shared Property mapSize As Size
    Public ReadOnly Property PositionWorld As PointF
        Get
            Dim point As New PointF
            Single.TryParse(_Pos_X, point.X)
            Single.TryParse(_Pos_Y, point.Y)
            Return point
        End Get
    End Property
    Public ReadOnly Property PositionScreen As PointF
        Get
            Dim point As New PointF
            Single.TryParse(_Pos_X, point.X)
            Single.TryParse(_Pos_Y, point.Y)
            point.X += _mapSize.Width / 2
            point.Y += _mapSize.Height / 2
            Return point
        End Get
    End Property
    Public ReadOnly Property PositionScreenDraw(ByVal zoomLvl As Integer) As PointF
        Get
            Dim point As New PointF
            Single.TryParse(_Pos_X, point.X)
            Single.TryParse(_Pos_Y, point.Y)
            point.X = (point.X + _mapSize.Width / 2) * zoomLvl / 100
            point.Y = (point.Y + _mapSize.Height / 2) * zoomLvl / 100
            Return point
        End Get
    End Property
    Public Property Pos_X As Double
    Public Property Pos_Y As Double
    Public Property Angle As Double
    Public Property Reverse As Boolean
    Public Property Wait As Boolean
    Public Property Cross As Boolean
    Public Property TurnStart As Boolean
    Public Property TurnEnd As Boolean
    Public Property Speed As Double
    Public Property generated As String
    Public Property turn As String
    Public Property dir As String
    Public Property ridgemarker As Double
    Public Property isSelected As Boolean
    Public ReadOnly Property ReverseTxt As String
        Get
            If _Reverse = True Then
                Return "1"
            Else
                Return "0"
            End If
        End Get
    End Property
    Public ReadOnly Property CrossTxt As String
        Get
            If _Cross = True Then
                Return "1"
            Else
                Return "0"
            End If
        End Get
    End Property
    Public ReadOnly Property WaitTxt As String
        Get
            If _Wait = True Then
                Return "1"
            Else
                Return "0"
            End If
        End Get
    End Property
    Public ReadOnly Property TurnStartTxt As String
        Get
            If _TurnStart = True Then
                Return "1"
            Else
                Return "0"
            End If
        End Get
    End Property
    Public ReadOnly Property TurnEndTxt As String
        Get
            If _TurnEnd = True Then
                Return "1"
            Else
                Return "0"
            End If
        End Get
    End Property
    Public ReadOnly Property generatedTXT As String
        Get
            If _generated = True Then
                Return "true"
            Else
                Return ""
            End If
        End Get
    End Property

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        If mapSize.IsEmpty = True Then
            mapSize = New Size(2048, 2048)
        End If
        AddHandler clsWaypoint.SelectionChanged, AddressOf Me.SelectionChangedHandler
    End Sub
    ''' <summary>
    ''' Find if this waypoint is selected (in range of point)
    ''' </summary>
    ''' <param name="point"></param>
    ''' <param name="range"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function selectWP(ByVal point As PointF, ByVal range As Double) As Boolean
        Dim xDist As Double
        Dim yDist As Double
        Dim dist As Double
        If clsWaypoint._isAnySelected = True Then
            RaiseEvent SelectionChanged(Nothing)
        End If

        xDist = point.X - Me.PositionScreen.X
        yDist = point.Y - Me.PositionScreen.Y
        dist = Math.Sqrt(xDist * xDist + yDist * yDist)

        If dist <= range Then
            RaiseEvent SelectionChanged(Me)
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' Set new position
    ''' </summary>
    ''' <param name="point"></param>
    ''' <remarks></remarks>
    Public Sub setNewPos(ByVal point As PointF)
        Me.Pos_X = point.X - mapSize.Width / 2
        Me.Pos_Y = point.Y - mapSize.Height / 2
    End Sub
    ''' <summary>
    ''' Set selected attribute
    ''' </summary>
    ''' <param name="wp"></param>
    ''' <remarks></remarks>
    Private Sub SelectionChangedHandler(ByRef wp As clsWaypoint)
        If wp Is Nothing Then
            Me._isSelected = False
            If clsWaypoint._isAnySelected = True Then clsWaypoint._isAnySelected = False
        Else
            If clsWaypoint._isAnySelected = False Then clsWaypoint._isAnySelected = True
            If Me.Equals(wp) Then
                Me._isSelected = True
            Else
                Me._isSelected = False
            End If
        End If
    End Sub
    ''' <summary>
    ''' Create XML from waypoint
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getXML(ByVal id As Integer) As XElement
        Dim myNam As String = "waypoint" & id
        Dim ConvSpeed As Double = Me.Speed
        If ConvSpeed < 1 Then
            ConvSpeed = ConvSpeed * 1000
        End If

        Dim el As New XElement(myNam)
        el.Add(New XAttribute("pos", Me.Pos_X.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) & " " & Me.Pos_Y.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)))
        el.Add(New XAttribute("angle", Me.Angle.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)))
        If Me.Reverse Then
            el.Add(New XAttribute("rev", Me.ReverseTxt))
        End If
        If Me.Wait Then
            el.Add(New XAttribute("wait", Me.WaitTxt))
        End If
        If Me.TurnStartTxt Then
            el.Add(New XAttribute("turnstart", Me.TurnStartTxt))
        End If
        If Me.TurnEnd Then
            el.Add(New XAttribute("turnend", Me.TurnEndTxt))
        End If
        If Me.Cross Then
            el.Add(New XAttribute("crossing", Me.CrossTxt))
        End If
        If Me.generated = "true" Then
            el.Add(New XAttribute("generated", Me.generatedTXT))
        End If
        If Me.ridgemarker > 0 Then
            el.Add(New XAttribute("ridgemarker", ridgemarker.ToString("0", System.Globalization.CultureInfo.InvariantCulture)))
        End If
        If Me.turn <> "" Then
            el.Add(New XAttribute("turn", Me.turn))
        End If
        If Me.dir <> "" Then
            el.Add(New XAttribute("dir", Me.dir))
        End If
        el.Add(New XAttribute("speed", ConvSpeed.ToString("0", System.Globalization.CultureInfo.InvariantCulture)))

        Return el
    End Function
    ''' <summary>
    ''' Destructor
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub Finalize()
        RemoveHandler clsWaypoint.SelectionChanged, AddressOf Me.SelectionChangedHandler
        MyBase.Finalize()
    End Sub
    ''' <summary>
    ''' Clone waypoint (create copy)
    ''' </summary>
    ''' <param name="dX">offset X</param>
    ''' <param name="dY">offset Y</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Clone(Optional ByVal dX As Single = 10.0, Optional ByVal dY As Single = 10.0) As clsWaypoint
        Dim wp As New clsWaypoint
        wp.Pos_X = Me.Pos_X + dX
        wp.Pos_Y = Me.Pos_Y + dY
        'wp.Angle = Me.Angle
        wp.Speed = Me.Speed
        'wp.Wait = Me.Wait
        wp.Reverse = Me.Reverse
        Me.isSelected = False
        wp.isSelected = True
        'wp.Cross = Me.Cross
        'Here is where we have to introduce the conditional. Is this related to the append waypoint
        Return wp
    End Function
    ''' <summary>
    ''' Force select this waypoint
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub forceSelect()
        RaiseEvent SelectionChanged(Me)
    End Sub
    ''' <summary>
    ''' Force unselect
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub forceUnselect()
        RaiseEvent SelectionChanged(Nothing)
    End Sub
End Class
