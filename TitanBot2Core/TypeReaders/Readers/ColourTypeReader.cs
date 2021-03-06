﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class ColourTypeReader : TypeReader
    {
        public override Task<TypeReaderResponse> Read(CmdContext context, string value)
        {
            var colour = Color.FromName(value);
            if (colour.IsKnownColor)
                return Task.FromResult(TypeReaderResponse.FromSuccess(colour));

            var input = (string)value.Clone();
            if (value.StartsWith("#"))
                input = value.Substring(1);

            int r = 0;
            int g = 0;
            int b = 0;

            int charsPerVal;

            if (input.Length == 3)
                charsPerVal = 1;
            else if (input.Length == 6)
                charsPerVal = 2;
            else
                return Task.FromResult(TypeReaderResponse.FromError($"`{value}` is not a valid colour"));

            if (!int.TryParse(input.Substring(0, charsPerVal), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out r) ||
                !int.TryParse(input.Substring(charsPerVal, charsPerVal), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out g) ||
                !int.TryParse(input.Substring(2 * charsPerVal, charsPerVal), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out b))
                return Task.FromResult(TypeReaderResponse.FromError($"`{value}` is not a valid colour"));

            return Task.FromResult(TypeReaderResponse.FromSuccess(Color.FromArgb(r, g, b)));

        }
    }
}
