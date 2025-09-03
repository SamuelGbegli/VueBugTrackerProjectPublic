// See https://aka.ms/new-console-template for more information
using Sodium;

Console.WriteLine("Hello, World!");

var hash = PasswordHash.ArgonHashString("Hello, World!");

Console.WriteLine(hash);
Console.WriteLine(PasswordHash.ArgonHashStringVerify(hash, "Hello, World!"));
Console.WriteLine(PasswordHash.ArgonHashStringVerify(hash, "HelloWorld"));