using System;

namespace Aurora.Timeline
{
    internal readonly struct CopyPosition
    {
        internal CopyPosition(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        public static CopyPosition Start => new CopyPosition();

        internal int Row { get; }

        internal int Column { get; }

        public CopyPosition Normalize(int endColumn)
        {
            return this.Column != endColumn ? this : new CopyPosition(this.Row + 1, 0);
        }

        private string DebuggerDisplay => $"[{this.Row}, {this.Column}]";
    }
}
