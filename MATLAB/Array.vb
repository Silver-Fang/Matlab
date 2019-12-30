﻿Public Interface IPlusable
	Function Plus(B As IPlusable) As IPlusable
End Interface
Public Interface IMinusable
	Function Minus(B As IMinusable) As IMinusable
End Interface
Public Class Array(Of T)
	Implements ICollection
	ReadOnly 本体 As Array

	Public ReadOnly Property Count As Integer Implements ICollection.Count
		Get
			Return 本体.Length
		End Get
	End Property

	Public ReadOnly Property IsSynchronized As Boolean Implements ICollection.IsSynchronized
		Get
			Return 本体.IsSynchronized
		End Get
	End Property

	Public ReadOnly Property SyncRoot As Object Implements ICollection.SyncRoot
		Get
			Return 本体.SyncRoot
		End Get
	End Property

	Private Sub 赋值递归(源数组 As Array, 目标数组 As Array, 各维长度 As Integer(), 总维数 As Byte, 当前维度 As Byte, 当前索引 As Integer())
		If 当前维度 < 总维数 - 1 Then
			For a As Integer = 0 To 各维长度(当前维度) - 1
				当前索引(当前维度) = a
				赋值递归(源数组, 目标数组, 各维长度, 总维数, 当前维度 + 1, 当前索引)
			Next
		Else
			For a As Integer = 0 To 各维长度(当前维度) - 1
				当前索引(当前维度) = a
				目标数组.SetValue(CType(源数组.GetValue(当前索引), T), 当前索引)
			Next
		End If
	End Sub
	Private Sub ToString递归(字符串 As Text.StringBuilder, 各维长度 As Integer(), 总维数 As Byte, 当前维度 As Byte, 当前索引 As Integer())
		If 当前维度 > 1 Then
			For a As Integer = 0 To 各维长度(当前维度) - 1
				当前索引(当前维度) = a
				ToString递归(字符串, 各维长度, 总维数, 当前维度 - 1, 当前索引)
			Next
		Else
			字符串.Append("(:,:")
			For c As Byte = 2 To 当前索引.GetUpperBound(0)
				字符串.Append(",").Append(当前索引(c))
			Next
			字符串.AppendLine(")")
			For a As Integer = 0 To 各维长度(0) - 1
				当前索引(0) = a
				For b As Integer = 0 To 各维长度(1) - 1
					当前索引(1) = b
					字符串.Append(本体.GetValue(当前索引)).Append(" ")
				Next
				字符串.AppendLine()
			Next
		End If
	End Sub
	''' <summary>
	''' 此构造会创建一个指定元素类型的新数组
	''' </summary>
	''' <param name="本体">源数组</param>
	Sub New(本体 As Array)
		Dim a As Integer() = 本体.Size
		Me.本体 = Array.CreateInstance(GetType(T), a)
		赋值递归(本体, Me.本体, a, 本体.Rank, 0, Zeros(Of Integer)(本体.Rank))
	End Sub
	''' <summary>
	''' 此构造会创建一个以给定对象为唯一元素的数组
	''' </summary>
	''' <param name="本体">给定对象</param>
	Sub New(本体 As Object)
		Me.本体 = {本体}
	End Sub
	''' <summary>
	''' 此构造不会创建新数组，只改变表示形式
	''' </summary>
	''' <param name="本体">源数组</param>
	Sub New(本体 As T())
		Me.本体 = 本体
	End Sub
	Sub New(ParamArray lengths As Integer())
		本体 = Array.CreateInstance(GetType(T), lengths)
	End Sub
	''' <summary>
	''' 此转换会创建一个新数组
	''' </summary>
	''' <param name="本体">待转换的数组</param>
	''' <returns>尺寸相同的新数组，但元素类型可能改变</returns>
	Public Shared Widening Operator CType(本体 As Array) As Array(Of T)
		Return New Array(Of T)(本体)
	End Operator
	''' <summary>
	''' 此转换不会创建新数组，只会改变表示形式
	''' </summary>
	''' <param name="本体">待转换的数组</param>
	''' <returns>和源数组是同一个数组，只是表示形式不同</returns>
	Public Shared Narrowing Operator CType(本体 As Array(Of T)) As Array
		Return 本体.本体
	End Operator
	''' <summary>
	''' 此转换创建一个只有本体一个元素的数组
	''' </summary>
	''' <param name="本体">唯一元素</param>
	''' <returns>单元素数组</returns>
	Public Shared Widening Operator CType(本体 As T) As Array(Of T)
		Return New Array(Of T)(本体)
	End Operator
	''' <summary>
	''' 此转换返回数组的第0个元素
	''' </summary>
	''' <param name="本体">数组</param>
	''' <returns>数组的第0个元素</returns>
	Public Shared Narrowing Operator CType(本体 As Array(Of T)) As T
		Return 本体.GetValue(0)
	End Operator
	''' <summary>
	''' 此转换不创建新数组，只改变表示形式
	''' </summary>
	''' <param name="本体">源数组</param>
	''' <returns>改变了表示形式的源数组</returns>
	Public Shared Widening Operator CType(本体 As T()) As Array(Of T)
		Return New Array(Of T)(本体)
	End Operator
	''' <summary>
	''' 此转换会创建一个新一维数组，为源数组的穷举
	''' </summary>
	''' <param name="本体">源数组</param>
	''' <returns>源数组的穷举</returns>
	Public Shared Narrowing Operator CType(本体 As Array(Of T)) As T()
		Return 本体.本体.Cast(Of T)
	End Operator
	Public Shared Operator +(A As Array(Of T), B As Array(Of T)) As Array(Of T)
		If GetType(T).GetInterfaces.Contains(GetType(IPlusable)) Then
			Return Bsxfun(Function(c As IPlusable, d As IPlusable) As T
							  Return c.Plus(d)
						  End Function, A, B)
		Else
			Return Bsxfun(Function(c As Double, d As Double) As T
							  Return DirectCast(c + d, Object)
						  End Function, A, B)
		End If
	End Operator

	Public Shared Operator -(A As Array(Of T), B As Array(Of T)) As Array(Of T)
		If GetType(T).GetInterfaces.Contains(GetType(IMinusable)) Then
			Return Bsxfun(Function(c As IMinusable, d As IMinusable) As T
							  Return c.Minus(d)
						  End Function, A, B)
		Else
			Return Bsxfun(Function(c As Double, d As Double) As T
							  Return DirectCast(c - d, Object)
						  End Function, A, B)
		End If
	End Operator
	Public Function GetValue(ParamArray indices As Integer()) As T
		Return 本体.GetValue(indices)
	End Function
	Public Function Cast(Of TOut)() As Array(Of TOut)
		Return New Array(Of TOut)(本体)
	End Function
	Public Sub SetValue(value As Object, ParamArray indices As Integer())
		本体.SetValue(value, indices)
	End Sub
	Public Function GetUpperBound(dimension As Integer) As Integer
		Return 本体.GetUpperBound(dimension)
	End Function
	Public Sub CopyTo(array As Array, index As Integer) Implements ICollection.CopyTo
		本体.CopyTo(array, index)
	End Sub

	Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
		Return 本体.GetEnumerator
	End Function
	Default Property Item(indices As Integer()) As T
		Get
			Return 本体.GetValue(indices)
		End Get
		Set(value As T)
			本体.SetValue(value, indices)
		End Set
	End Property
	Function Size() As Integer()
		Return Elmat.Size(本体)
	End Function
	Function Size([dim] As Byte) As Integer
		Return Elmat.Size(本体, [dim])
	End Function
	Public Overrides Function ToString() As String
		Dim a As New Text.StringBuilder
		Select Case 本体.Rank
			Case 1
				For Each b As T In 本体
					a.Append(b.ToString).Append(" ")
				Next
			Case 2
				a.AppendLine("列为第0维，行为第1维")
				For b As Integer = 0 To 本体.GetUpperBound(0)
					For c As Integer = 0 To 本体.GetUpperBound(1)
						a.Append(本体.GetValue(b, c).ToString).Append(" ")
					Next
					a.AppendLine()
				Next
			Case Else
				a.AppendLine("列为第0维，行为第1维")
				ToString递归(a, Size, 本体.Rank, 本体.Rank - 1, Zeros(Of Integer)(本体.Rank))
		End Select
		Return a.ToString
	End Function
End Class