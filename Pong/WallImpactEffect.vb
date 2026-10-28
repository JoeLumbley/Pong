Public Class WallImpactEffect

    Private Const Duration As Double = 0.1

    Private wallImpactPoint As PointF
    Private age As Double = Duration

    Public Sub Trigger(location As PointF)

        wallImpactPoint = location
        age = 0

    End Sub

    Public Sub Update(dt As Double)

        If age < Duration Then

            age += dt

            If age > Duration Then
                age = Duration
            End If

        End If

    End Sub

    Public ReadOnly Property IsActive As Boolean
        Get
            Return age < Duration
        End Get
    End Property

    Public ReadOnly Property WallImpactPt As PointF
        Get
            Return wallImpactPoint
        End Get
    End Property

    Public ReadOnly Property Alpha As Single
        Get
            Return 1.0F - CSng(age / Duration)
        End Get
    End Property

    Public ReadOnly Property Progress As Single
        Get
            Return CSng(age / Duration)
        End Get
    End Property

End Class