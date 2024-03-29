﻿using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;




/*
사용법
GoogleSignatureVerify gsv = new GoogleSignatureVerify("발급받은 API키");
if (gsv.Verify("결제데이터", 사인데이터))
{
//성공시 처리
}
*/

public class GoogleSignatureVerify
{
    RSAParameters _rsaKeyInfo;

    public GoogleSignatureVerify(String GooglePublicKey)
    {
        RsaKeyParameters rsaParameters = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(GooglePublicKey));

        byte[] rsaExp = rsaParameters.Exponent.ToByteArray();
        byte[] Modulus = rsaParameters.Modulus.ToByteArray();

        // Microsoft RSAParameters modulo wants leading zero's removed so create new array with leading zero's removed
        int Pos = 0;
        for (int i = 0; i < Modulus.Length; i++)
        {
            if (Modulus[i] == 0)
            {
                Pos++;
            }
            else
            {
                break;
            }
        }

        byte[] rsaMod = new byte[Modulus.Length - Pos];
        Array.Copy(Modulus, Pos, rsaMod, 0, Modulus.Length - Pos);

        // Fill the Microsoft parameters
        _rsaKeyInfo = new RSAParameters()
        {
            Exponent = rsaExp,
            Modulus = rsaMod
        };
    }


    public bool Verify(String Message, String Signature)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(_rsaKeyInfo);
            return rsa.VerifyData(Encoding.ASCII.GetBytes(Message), "SHA1", Convert.FromBase64String(Signature));
        }
    }
}