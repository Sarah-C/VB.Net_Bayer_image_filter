Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO

Class Main

    Public Shared Sub Main(ByVal args() As String)
        Dim m As New Main(args)
    End Sub

    Public Sub New(ByVal args() As String)
        Dim actioned As Boolean = False
        Dim name As String = System.Diagnostics.Process.GetCurrentProcess().ProcessName
        Console.WriteLine("")
        If args.Length = 0 OrElse
            (args.Length = 1 And
                (args(0).ToLower = "/help" Or args(0).ToLower() = "/?")
            ) Then
            Console.WriteLine("Bayeriser")
            Console.WriteLine("")
            Console.WriteLine("Instructions:")
            Console.WriteLine("")
            Console.WriteLine($" {name} /? or /help          : This help")
            Console.WriteLine($" {name} /split ""imageName""   : Splits file into 3: R, G, B")
            Console.WriteLine($" {name} /makeraw ""imageName"" : Uses the 3 files above to make RAW file")
            Console.WriteLine($" {name} /unraw ""imageName""   : Uses the raw file to make final image")
            Console.WriteLine($" {name} /all ""imageName""     : Do all steps above")
            Return
        End If

        If args.Length < 2 Or args.Length > 3 Then Console.WriteLine("Not the correct number of arguments.") : Return
        Dim command As String = args(0).ToLower()
        Dim file As String = args(1)
        If command = "/split" Then splitBayer(file) : actioned = True
        If command = "/makeraw" Then makeRaw(file) : actioned = True
        If command = "/unraw" Then processRaw(file) : actioned = True
        If command = "/all" Then
            splitBayer(file)
            Console.WriteLine("")
            makeRaw(file)
            Console.WriteLine("")
            processRaw(file)
            actioned = True
        End If
        If Not actioned Then
            Console.WriteLine($"The command {command} was not recognised.")
            Console.WriteLine($"Use:")
            Console.WriteLine($"     {name} /?")
            Console.WriteLine($"To see instructions.")
            Return
        End If
    End Sub

    Public Function get32BPP(ByVal source As Bitmap) As Bitmap
        Dim width As Integer = source.Width
        Dim height As Integer = source.Height
        Dim source32BPP As New Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)
        Dim source32BPPG = Graphics.FromImage(source32BPP)
        source32BPPG.DrawImageUnscaled(source, 0, 0)
        source32BPPG.Dispose()
        Return source32BPP
    End Function

    Public Sub makeRaw(ByVal imgName As String)
        imgName = Path.GetFileNameWithoutExtension(imgName)
        Dim red As Bitmap = Nothing
        Dim green As Bitmap = Nothing
        Dim blue As Bitmap = Nothing
        Console.WriteLine($"Loading {imgName}_red.png, {imgName}_green.png, {imgName}_blue.png")
        Try
            red = New Bitmap(imgName & "_red.png")
            green = New Bitmap(imgName & "_green.png")
            blue = New Bitmap(imgName & "_blue.png")
        Catch ex As Exception
            Console.WriteLine("Could not open the three colour files.")
            Return
        End Try
        Dim w As Integer = red.Width - 2
        Dim h As Integer = red.Height
        Dim dest As Bitmap = New Bitmap(w * 2, h * 2, Imaging.PixelFormat.Format32bppArgb)

        For x As Integer = 0 To w - 1
            For y As Integer = 0 To h - 1
                Dim r As Byte = red.GetPixel(x, y).R
                dest.SetPixel(x * 2 + 1, y * 2 + 1, Color.FromArgb(255, r, 0, 0))

                Dim g As Byte = green.GetPixel(x, y * 2).G
                dest.SetPixel(x * 2 + 1, y * 2, Color.FromArgb(255, 0, g, 0))

                g = green.GetPixel(x, y * 2 + 1).G
                dest.SetPixel(x * 2, y * 2 + 1, Color.FromArgb(255, 0, g, 0))

                Dim b As Byte = blue.GetPixel(x, y).B
                dest.SetPixel(x * 2, y * 2, Color.FromArgb(255, 0, 0, b))

            Next
        Next
        dest.Save(imgName & "_raw.png", ImageFormat.Png)
        Console.WriteLine($"{imgName}_raw.png has been created.")
    End Sub

    Public Sub splitBayer(ByVal imgName As String)
        Dim source As Bitmap = Nothing
        Console.WriteLine($"Loading {imgName}")
        Try
            source = get32BPP(New Bitmap(imgName))
        Catch ex As Exception
            Console.WriteLine("Could not open the image file.")
            Return
        End Try
        Dim name As String = Path.GetFileNameWithoutExtension(imgName)
        Dim w As Integer = source.Width
        Dim h As Integer = source.Height
        Dim red As New Bitmap(w / 2, h / 2, Imaging.PixelFormat.Format32bppArgb)
        Dim rX As Integer = 0
        Dim rY As Integer = 0
        Dim green As New Bitmap(w / 2, h, Imaging.PixelFormat.Format32bppArgb)
        Dim gX As Integer = 0
        Dim gY As Integer = 0
        Dim blue As New Bitmap(w / 2, h / 2, Imaging.PixelFormat.Format32bppArgb)
        Dim bX As Integer = 0
        Dim bY As Integer = 0

        For x As Integer = 0 To w - 2 Step 2
            For y As Integer = 0 To h - 1 Step 2

                Dim r As Byte = source.GetPixel(x + 1, y + 1).R
                red.SetPixel(x / 2, y / 2, Color.FromArgb(255, r, 0, 0))

                Dim g As Byte = source.GetPixel(x + 1, y).G
                green.SetPixel(x / 2, y, Color.FromArgb(255, 0, g, 0))

                g = source.GetPixel(x, y + 1).G
                green.SetPixel(x / 2, y + 1, Color.FromArgb(255, 0, g, 0))

                Dim b As Byte = source.GetPixel(x + 1, y).B
                blue.SetPixel(x / 2, y / 2, Color.FromArgb(255, 0, 0, b))

            Next
        Next

        red.Save(name & "_red.png", ImageFormat.Png)
        green.Save(name & "_green.png", ImageFormat.Png)
        blue.Save(name & "_blue.png", ImageFormat.Png)
        Console.WriteLine($"{name}_red.png, {name}_green.png, and {name}_blue.png have been created.")
    End Sub

    Public Sub processRaw(ByVal imgName As String)
        imgName = Path.GetFileNameWithoutExtension(imgName)
        Dim raw As Bitmap = Nothing
        Console.WriteLine($"Loading {imgName}_raw.png")
        Try
            raw = get32BPP(New Bitmap(imgName & "_raw.png"))
        Catch ex As Exception
            Console.WriteLine("Could not open the raw file.")
            Return
        End Try
        Dim w As Integer = raw.Width
        Dim h As Integer = raw.Height
        Dim dest As Bitmap = New Bitmap(w, h, Imaging.PixelFormat.Format32bppArgb)
        Dim destG As Graphics = Graphics.FromImage(dest)

        destG.DrawImageUnscaled(raw, 0, 0)

        Dim mode As Integer = 2

        If mode = 1 Then
            For x As Integer = 0 To w - 1 Step 2
                For y As Integer = 0 To h - 1 Step 2
                    Dim r As Byte = raw.GetPixel(x + 1, y + 1).R
                    Dim b As Byte = raw.GetPixel(x, y).B
                    Dim g1 As Integer = raw.GetPixel(x + 1, y).G
                    Dim g2 As Integer = raw.GetPixel(x, y + 1).G

                    Dim tl As Color = Color.FromArgb(255, r, (g1 + g2) / 2, b)
                    Dim tr As Color = Color.FromArgb(255, r, (g1 + g2) / 2, b)
                    Dim bl As Color = Color.FromArgb(255, r, (g1 + g2) / 2, b)
                    Dim br As Color = Color.FromArgb(255, r, (g1 + g2) / 2, b)

                    dest.SetPixel(x, y, tl)
                    dest.SetPixel(x + 1, y, tr)
                    dest.SetPixel(x, y + 1, bl)
                    dest.SetPixel(x + 1, y + 1, br)

                Next
            Next
        End If

        If mode = 2 Then

            For x As Integer = 0 To w - 4 Step 2
                For y As Integer = 0 To h - 3 Step 2

                    Dim lt As Decimal = dest.GetPixel(x, y).B
                    Dim rt As Decimal = dest.GetPixel(x + 2, y).B
                    Dim lb As Decimal = dest.GetPixel(x, y + 2).B
                    Dim rb As Decimal = dest.GetPixel(x + 2, y + 2).B

                    Dim mt As Integer = ((lt + rt) / 2.0)
                    Dim lm As Integer = ((lt + lb) / 2.0)
                    Dim mm As Integer = ((lt + rt + lb + rb) / 4.0)

                    Dim cmt As Color = dest.GetPixel(x + 1, y)
                    Dim clm As Color = dest.GetPixel(x, y + 1)
                    Dim cmm As Color = dest.GetPixel(x + 1, y + 1)

                    Dim ncmt As Color = Color.FromArgb(255, cmt.R, cmt.G, mt)
                    Dim nclm As Color = Color.FromArgb(255, clm.R, clm.G, lm)
                    Dim ncmm As Color = Color.FromArgb(255, cmm.R, cmm.G, mm)

                    dest.SetPixel(x + 1, y, ncmt)
                    dest.SetPixel(x, y + 1, nclm)
                    dest.SetPixel(x + 1, y + 1, ncmm)
                Next
            Next

            For x As Integer = 1 To w - 4 Step 2
                For y As Integer = 1 To h - 3 Step 2

                    Dim lt As Decimal = dest.GetPixel(x, y).R
                    Dim rt As Decimal = dest.GetPixel(x + 2, y).R
                    Dim lb As Decimal = dest.GetPixel(x, y + 2).R
                    Dim rb As Decimal = dest.GetPixel(x + 2, y + 2).R

                    Dim mt As Integer = ((lt + rt) / 2.0)
                    Dim lm As Integer = ((lt + lb) / 2.0)
                    Dim mm As Integer = ((lt + rt + lb + rb) / 4.0)

                    Dim cmt As Color = dest.GetPixel(x + 1, y)
                    Dim clm As Color = dest.GetPixel(x, y + 1)
                    Dim cmm As Color = dest.GetPixel(x + 1, y + 1)

                    Dim ncmt As Color = Color.FromArgb(255, mt, cmt.G, cmt.B)
                    Dim nclm As Color = Color.FromArgb(255, lm, clm.G, clm.B)
                    Dim ncmm As Color = Color.FromArgb(255, mm, cmm.G, cmm.B)

                    dest.SetPixel(x + 1, y, ncmt)
                    dest.SetPixel(x, y + 1, nclm)
                    dest.SetPixel(x + 1, y + 1, ncmm)
                Next
            Next

            For x As Integer = 0 To w - 4 Step 2
                For y As Integer = 0 To h - 3 Step 2
                    Dim mt As Decimal = dest.GetPixel(x + 1, y).G
                    Dim lm As Decimal = dest.GetPixel(x, y + 1).G
                    Dim rm As Decimal = dest.GetPixel(x + 2, y + 1).G
                    Dim bm As Decimal = dest.GetPixel(x + 1, y + 2).G
                    Dim mm As Integer = ((mt + lm + rm + bm) / 4.0)
                    Dim cmm As Color = dest.GetPixel(x + 1, y + 1)
                    Dim ncmm As Color = Color.FromArgb(255, cmm.R, mm, cmm.B)
                    dest.SetPixel(x + 1, y + 1, ncmm)
                Next
            Next

            For x As Integer = 1 To w - 4 Step 2
                For y As Integer = 1 To h - 3 Step 2
                    Dim mt As Decimal = dest.GetPixel(x + 1, y).G
                    Dim lm As Decimal = dest.GetPixel(x, y + 1).G
                    Dim rm As Decimal = dest.GetPixel(x + 2, y + 1).G
                    Dim bm As Decimal = dest.GetPixel(x + 1, y + 2).G
                    Dim mm As Integer = ((mt + lm + rm + bm) / 4.0)
                    Dim cmm As Color = dest.GetPixel(x + 1, y + 1)
                    Dim ncmm As Color = Color.FromArgb(255, cmm.R, mm, cmm.B)
                    dest.SetPixel(x + 1, y + 1, ncmm)
                Next
            Next

        End If
        dest.Save(imgName & "_.png", ImageFormat.Png)
        Console.WriteLine($"Created ""original"" image {imgName}_.png from raw image.")
    End Sub




End Class
