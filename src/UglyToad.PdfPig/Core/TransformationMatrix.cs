﻿namespace UglyToad.PdfPig.Core
{
    using System;
    using System.Diagnostics.Contracts;
    using Geometry;

    /// <summary>
    /// Specifies the conversion from the transformed coordinate space to the original untransformed coordinate space.
    /// </summary>
    public struct TransformationMatrix
    {
        /// <summary>
        /// The default <see cref="TransformationMatrix"/>.
        /// </summary>
        public static TransformationMatrix Identity = new TransformationMatrix(1,0,0,
            0,1,0,
            0,0,1);
        
        /// <summary>
        /// Create a new <see cref="TransformationMatrix"/> with the X and Y translation values set.
        /// </summary>
        public static TransformationMatrix GetTranslationMatrix(decimal x, decimal y) => new TransformationMatrix(1, 0, 0,
            0, 1, 0,
            x, y, 1);

        private readonly decimal row1;
        private readonly decimal row2;
        private readonly decimal row3;

        /// <summary>
        /// The value at (0, 0) - The scale for the X dimension.
        /// </summary>
        public readonly decimal A;
        /// <summary>
        /// The value at (0, 1).
        /// </summary>
        public readonly decimal B;
        /// <summary>
        /// The value at (1, 0).
        /// </summary>
        public readonly decimal C;
        /// <summary>
        /// The value at (1, 1) - The scale for the Y dimension.
        /// </summary>
        public readonly decimal D;
        /// <summary>
        /// The value at (2, 0) - translation in X.
        /// </summary>
        public readonly decimal E;
        /// <summary>
        /// The value at (2, 1) - translation in Y.
        /// </summary>
        public readonly decimal F;
        
        /// <summary>
        /// Get the value at the specific row and column.
        /// </summary>
        public decimal this[int row, int col]
        {
            get
            {
                if (row >= Rows)
                {
                    throw new ArgumentOutOfRangeException(nameof(row), $"The transformation matrix only contains {Rows} rows and is zero indexed, you tried to access row {row}.");
                }

                if (row < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(row), "Cannot access negative rows in a matrix.");
                }

                if (col >= Columns)
                {
                    throw new ArgumentOutOfRangeException(nameof(col), $"The transformation matrix only contains {Columns} columns and is zero indexed, you tried to access column {col}.");
                }

                if (col < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(col), "Cannot access negative columns in a matrix.");
                }

                switch (row)
                {
                    case 0:
                        {
                            switch (col)
                            {
                                case 0:
                                    return A;
                                case 1:
                                    return B;
                                case 2:
                                    return row1;
                                default:
                                    throw new ArgumentOutOfRangeException($"Trying to access {row}, {col} which was not in the value array.");
                            }
                        }
                    case 1:
                        {
                            switch (col)
                            {
                                case 0:
                                    return C;
                                case 1:
                                    return D;
                                case 2:
                                    return row2;
                                default:
                                    throw new ArgumentOutOfRangeException($"Trying to access {row}, {col} which was not in the value array.");
                            }
                        }
                    case 2:
                        {
                            switch (col)
                            {
                                case 0:
                                    return E;
                                case 1:
                                    return F;
                                case 2:
                                    return row3;
                                default:
                                    throw new ArgumentOutOfRangeException($"Trying to access {row}, {col} which was not in the value array.");
                            }
                        }
                    default:
                        throw new ArgumentOutOfRangeException($"Trying to access {row}, {col} which was not in the value array.");
                }
            }
        }

        /// <summary>
        /// The number of rows in the matrix.
        /// </summary>
        public const int Rows = 3;
        /// <summary>
        /// The number of columns in the matrix.
        /// </summary>
        public const int Columns = 3;

        /// <summary>
        /// Create a new <see cref="TransformationMatrix"/>.
        /// </summary>
        /// <param name="value">The 9 values of the matrix.</param>
        public TransformationMatrix(decimal[] value) : this(value[0], value[1], value[2], value[3], value[4], value[5], value[6], value[7], value[8])
        {
        }

        /// <summary>
        /// Create a new <see cref="TransformationMatrix"/>.
        /// </summary>
        public TransformationMatrix(decimal a, decimal b, decimal r1, decimal c, decimal d, decimal r2, decimal e, decimal f, decimal r3)
        {
            A = a;
            B = b;
            row1 = r1;
            C = c;
            D = d;
            row2 = r2;
            E = e;
            F = f;
            row3 = r3;
        }

        /// <summary>
        /// Transform a point using this transformation matrix.
        /// </summary>
        /// <param name="original">The original point.</param>
        /// <returns>A new point which is the result of applying this transformation matrix.</returns>
        [Pure]
        public PdfPoint Transform(PdfPoint original)
        {
            var x = A * original.X + C * original.Y + E;
            var y = B * original.X + D * original.Y + F;

            return new PdfPoint(x, y);
        }

        /// <summary>
        /// Transform an X coordinate using this transformation matrix.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <returns>The transformed X coordinate.</returns>
        [Pure]
        internal decimal TransformX(decimal x)
        {
            var xt = A * x + C * 0 + E;

            return xt;
        }

        /// <summary>
        /// Transform a vector using this transformation matrix.
        /// </summary>
        /// <param name="original">The original vector.</param>
        /// <returns>A new vector which is the result of applying this transformation matrix.</returns>
        [Pure]
        internal PdfVector Transform(PdfVector original)
        {
            var x = A * original.X + C * original.Y + E;
            var y = B * original.X + D * original.Y + F;

            return new PdfVector(x, y);
        }

        /// <summary>
        /// Transform a rectangle using this transformation matrix.
        /// </summary>
        /// <param name="original">The original rectangle.</param>
        /// <returns>A new rectangle which is the result of applying this transformation matrix.</returns>
        [Pure]
        public PdfRectangle Transform(PdfRectangle original)
        {
            return new PdfRectangle(
                Transform(original.TopLeft),
                Transform(original.TopRight),
                Transform(original.BottomLeft),
                Transform(original.BottomRight)
            );
        }

        [Pure]
        internal TransformationMatrix Translate(decimal x, decimal y)
        {
            var a = A;
            var b = B;
            var r1 = row1;

            var c = C;
            var d = D;
            var r2 = row2;

            var e = (x * A) + (y * C) + E;
            var f = (x * B) + (y * D) + F;
            var r3 = (x * row1) + (y * row2) + row3;

            return new TransformationMatrix(a, b, r1,
                c, d, r2,
                e, f, r3);
        }

        /// <summary>
        /// Create a new <see cref="TransformationMatrix"/> from the 6 values provided in the default PDF order.
        /// </summary>
        public static TransformationMatrix FromValues(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f)
            => new TransformationMatrix(a, b, 0, c, d, 0, e, f, 1);

        /// <summary>
        /// Create a new <see cref="TransformationMatrix"/> from the 4 values provided in the default PDF order.
        /// </summary>
        public static TransformationMatrix FromValues(decimal a, decimal b, decimal c, decimal d)
            => new TransformationMatrix(a, b, 0, c, d, 0, 0, 0, 1);

        /// <summary>
        /// Create a new <see cref="TransformationMatrix"/> from the values.
        /// </summary>
        /// <param name="values">Either all 9 values of the matrix, 6 values in the default PDF order or the 4 values of the top left square.</param>
        /// <returns></returns>
        public static TransformationMatrix FromArray(decimal[] values)
        {
            if (values.Length == 9)
            {
                return new TransformationMatrix(values);
            }

            if (values.Length == 6)
            {
                return new TransformationMatrix(values[0], values[1], 0,
                    values[2], values[3], 0,
                    values[4], values[5], 1);
            }

            if (values.Length == 4)
            {
                return new TransformationMatrix(values[0], values[1], 0,
                    values[2], values[3], 0,
                    0, 0, 1);
            }

            throw new ArgumentException("The array must either define all 9 elements of the matrix or all 6 key elements. Instead array was: " + values);
        }

        /// <summary>
        /// Multiplies one transformation matrix by another without modifying either matrix. Order is: (this * matrix).
        /// </summary>
        /// <param name="matrix">The matrix to multiply</param>
        /// <returns>The resulting matrix.</returns>
        [Pure]
        public TransformationMatrix Multiply(TransformationMatrix matrix)
        {
            var a = (A * matrix.A) + (B * matrix.C) + (row1 * matrix.E);
            var b = (A * matrix.B) + (B * matrix.D) + (row1 * matrix.F);
            var r1 = (A * matrix.row1) + (B * matrix.row2) + (row1 * matrix.row3);

            var c = (C * matrix.A) + (D * matrix.C) + (row2 * matrix.E);
            var d = (C * matrix.B) + (D * matrix.D) + (row2 * matrix.F);
            var r2 = (C * matrix.row1) + (D * matrix.row2) + (row2 * matrix.row3);

            var e = (E * matrix.A) + (F * matrix.C) + (row3 * matrix.E);
            var f = (E * matrix.B) + (F * matrix.D) + (row3 * matrix.F);
            var r3 = (E * matrix.row1) + (F * matrix.row2) + (row3 * matrix.row3);

            return new TransformationMatrix(a, b, r1, 
                c, d, r2, 
                e, f, r3);
        }

        /// <summary>
        /// Multiplies the matrix by a scalar value without modifying this matrix.
        /// </summary>
        /// <param name="scalar">The value to multiply.</param>
        /// <returns>A new matrix which is multiplied by the scalar value.</returns>
        [Pure]
        public TransformationMatrix Multiply(decimal scalar)
        {
            return new TransformationMatrix(A * scalar, B * scalar, row1 * scalar,
                C * scalar, D * scalar, row2 * scalar,
                E * scalar, F * scalar, row3 * scalar);
        }

        /// <summary>
        /// Get the X scaling component of the current matrix.
        /// </summary>
        /// <returns></returns>
        internal decimal GetScalingFactorX()
        {
            var xScale = A;

            /*
             * BM: if the trm is rotated, the calculation is a little more complicated
             *
             * The rotation matrix multiplied with the scaling matrix is:
             * (   x   0   0)    ( cos  sin  0)    ( x*cos x*sin   0)
             * (   0   y   0) *  (-sin  cos  0)  = (-y*sin y*cos   0)
             * (   0   0   1)    (   0    0  1)    (     0     0   1)
             *
             * So, if you want to deduce x from the matrix you take
             * M(0,0) = x*cos and M(0,1) = x*sin and use the theorem of Pythagoras
             *
             * sqrt(M(0,0)^2+M(0,1)^2) =
             * sqrt(x2*cos2+x2*sin2) =
             * sqrt(x2*(cos2+sin2)) = (here is the trick cos2+sin2 = 1)
             * sqrt(x2) =
             * abs(x)
             */
            if (!(B == 0m && C == 0m))
            {
                xScale = (decimal)Math.Sqrt((double)(A * A + B * B));
            }

            return xScale;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is TransformationMatrix m))
            {
                return false;
            }

            return Equals(this, m);
        }

        /// <summary>
        /// Determines whether 2 transformation matrices are equal.
        /// </summary>
        public static bool Equals(TransformationMatrix a, TransformationMatrix b)
        {
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    if (a[i, j] != b[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 472622392;
            hashCode = hashCode * -1521134295 + row1.GetHashCode();
            hashCode = hashCode * -1521134295 + row2.GetHashCode();
            hashCode = hashCode * -1521134295 + row3.GetHashCode();
            hashCode = hashCode * -1521134295 + A.GetHashCode();
            hashCode = hashCode * -1521134295 + B.GetHashCode();
            hashCode = hashCode * -1521134295 + C.GetHashCode();
            hashCode = hashCode * -1521134295 + D.GetHashCode();
            hashCode = hashCode * -1521134295 + E.GetHashCode();
            hashCode = hashCode * -1521134295 + F.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{A}, {B}, {row1}\r\n{C}, {D}, {row2}\r\n{E}, {F}, {row3}";
        }
    }
}
