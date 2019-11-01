using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace decelerate.Utils.JWT
{
    public class JWT<T> where T: class
    {
        public JWT(string key, JWTAlgorithm algorithm)
        {
            _algorithm = algorithm;

            /* Create HMAC instance: */
            switch (_algorithm)
            {
                case JWTAlgorithm.HS256:
                    var keyBytes = Encoding.UTF8.GetBytes(key);
                    _hmac = new HMACSHA256(keyBytes);
                    break;
                /* Additional algorithms will be implemented here in future versions. */
            }

            /* Create serializer settings: */
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }
        public string Encode(T payload)
        {
            /* Create and encode header: */
            var header = new JWTHeader("JWT", _algorithm.GetString());
            var headerEncoded = EncodeObject(header, null);
            /* Encode payload: */
            var payloadEncoded = EncodeObject(payload, _settings);
            /* Create signature: */
            var signatureEncoded = (_algorithm == JWTAlgorithm.None) ? "" : CreateSignature(headerEncoded, payloadEncoded);
            /* Return everything: */
            return headerEncoded + "." + payloadEncoded + "." + signatureEncoded;
        }

        public T Decode(string token, out string errorMessage)
        {
            /* Check token: */
            if (token == null)
            {
                errorMessage = "token is null";
                return null;
            }
            /* Split the token into parts: */
            var tokenParts = token.Split(".");
            if (tokenParts.Length != 3)
            {
                errorMessage = "token has to contain exactly three parts separated by a period";
                return null;
            }
            /* Base64-decode and utf8-decode the first two parts: */
            var jsonParts = new List<string>();
            for (var i=0; i < 2; ++i)
            {
                /* Base64-decoding: */
                Byte[] tokenBytes;
                try
                {
                    tokenBytes = WebEncoders.Base64UrlDecode(tokenParts[i]);
                }
                catch (FormatException)
                {
                    errorMessage = $"base64-decoding of part {i} failed";
                    return null;
                }
                /* Utf8-decoding: */
                try
                {
                    jsonParts.Add(Encoding.UTF8.GetString(tokenBytes));
                }
                catch (ArgumentNullException)
                {
                    errorMessage = $"utf8-decoding of part {i} failed with ArgumentNullException";
                    return null;
                }
                catch (ArgumentException)
                {
                    errorMessage = $"utf8-decoding of part {i} failed with ArgumentException";
                    return null;
                }
            }
            /* JSON-decode and check header: */
            JWTHeader header;
            try
            {
                header = (JWTHeader)JsonConvert.DeserializeObject(jsonParts[0], typeof(JWTHeader));
            }
            catch (JsonReaderException)
            {
                errorMessage = "JSON-decoding the header failed";
                return null;
            }
            if (header.typ != "JWT")
            {
                errorMessage = "invalid JWT type";
                return null;
            }
            /* Check signature: */
            switch (header.alg)
            {
                case "HS256":
                    /* SHA256 HMAC */
                    var correctSignature = CreateSignature(tokenParts[0], tokenParts[1]);
                    if (correctSignature != tokenParts[2])
                    {
                        errorMessage = "invalid signature";
                        return null;
                    }
                    break;
                case "none":
                    /* no signature */
                    break;
                /* ^- These two algorithms are mandatory.
                 * Additional ones will be implemented here in future versions.
                 */
                default:
                    /* unknown algorithm */
                    errorMessage = "unknown JWT signature algorithm";
                    return null;
            }
            /* Finally, JSON-decode the payload: */
            T payload;
            try
            {
                payload = (T)JsonConvert.DeserializeObject(jsonParts[1], _settings);
            }
            catch (JsonReaderException)
            {
                errorMessage = "JSON-decoding the payload failed";
                return null;
            }
            /* ... and return the result: */
            errorMessage = "";
            return payload;
        }

        private string EncodeObject(Object obj, JsonSerializerSettings settings)
        {
            /* JSON-encode: */
            var json = JsonConvert.SerializeObject(obj, settings);
            /* Convert into raw bytes: */
            var bytes = Encoding.UTF8.GetBytes(json);
            /* base64-encode: */
            return WebEncoders.Base64UrlEncode(bytes);
        }

        private string CreateSignature(string headerEncoded, string payloadEncoded)
        {
            var stringToSign = headerEncoded + "." + payloadEncoded;
            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);
            var signature = _hmac.ComputeHash(bytesToSign);
            return WebEncoders.Base64UrlEncode(signature);
        }

        private readonly JWTAlgorithm _algorithm;
        private readonly HMACSHA256 _hmac;
        private readonly JsonSerializerSettings _settings;
    }
}
