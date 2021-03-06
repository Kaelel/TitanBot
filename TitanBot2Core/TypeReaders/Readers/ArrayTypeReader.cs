﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public static class ArrayTypeReader
    {
        public static IEnumerable<TypeReader> GetReaders(Type type, Func<Type, IEnumerable<TypeReader>> getReaders)
        {
            Type baseType = type.GetElementType();
            var constructor = typeof(ArrayTypeReader<>).MakeGenericType(baseType).GetTypeInfo().DeclaredConstructors.First();
            var readers = getReaders(baseType);
            return readers.Select(r => (TypeReader)constructor.Invoke(new object[] { type, r }));
        }
    }
    public class ArrayTypeReader<T> : TypeReader
    {
        private readonly Type _arrayType;
        private readonly TypeReader _parser;

        public ArrayTypeReader(Type type, TypeReader parser)
        {
            _arrayType = type;
            _parser = parser;
        }

        public override async Task<TypeReaderResponse> Read(CmdContext context, string value)
        {
            var values = new List<T>();

            if (_parser == null)
                return TypeReaderResponse.FromError($"No reader found for `{typeof(T)}`");

            foreach (var item in value.Split(','))
            {
                var response = await _parser?.Read(context, item.Trim());
                if (response.IsSuccess)
                    values.Add((T)response.Best);
                else
                    return TypeReaderResponse.FromError($"`{item.Trim()}` is not a valid `{typeof(T).Name}`");
            }

            return TypeReaderResponse.FromSuccess(values.ToArray());
        }
    }
}
