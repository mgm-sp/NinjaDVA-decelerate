using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;

/*
 * Let's ignore all available libraries and implement JWT on our own because that's always
 * a good idea with crypto stuff, right?
 */
namespace decelerate.Utils.JWT
{
    public class JWT<T> where T: class
    {
        public JWT(string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            hmac = new HMACSHA256(keyBytes);
        }
        public string Encode(T payload)
        {
            /* Create and encode header: */
            var header = new JWTHeader("JWT", "HS256");
            var headerEncoded = EncodeObject(header);
            /* Encode payload: */
            var payloadEncoded = EncodeObject(payload);
            /* Create signature: */
            var signatureEncoded = CreateSignature(headerEncoded, payloadEncoded);
            /* Return everything: */
            return headerEncoded + "." + payloadEncoded + "." + signatureEncoded;
        }

        public T Decode(string token, out string errorMessage)
        {
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
            if (header.alg != "HS256" && header.alg != "none")
            {
                /* These two algorithms are mandatory, so of course we only implemented them, but very carefully. */
                errorMessage = "invalid JWT algorithm";
                return null;
            }
            /* Check signature: */
            if (header.alg == "HS256")
            {
                var correctSignature = CreateSignature(tokenParts[0], tokenParts[1]);
                if (correctSignature != tokenParts[2])
                {
                    errorMessage = "invalid signature";
                    return null;
                }
            }
            /* Finally, JSON-decode the payload: */
            T payload;
            try
            {
                payload = (T)JsonConvert.DeserializeObject(jsonParts[1], typeof(T));
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

        private string EncodeObject(Object obj)
        {
            /* JSON-encode: */
            var json = JsonConvert.SerializeObject(obj);
            /* Convert into raw bytes: */
            var bytes = Encoding.UTF8.GetBytes(json);
            /* base64-encode: */
            return WebEncoders.Base64UrlEncode(bytes);
        }

        private string CreateSignature(string headerEncoded, string payloadEncoded)
        {
            var stringToSign = headerEncoded + "." + payloadEncoded;
            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);
            var signature = hmac.ComputeHash(bytesToSign);
            return WebEncoders.Base64UrlEncode(signature);
        }

        private readonly HMACSHA256 hmac;
    }
}
