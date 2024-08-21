using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Xml.Linq;
using static LINQ_Assignment_01.ListGenerators;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace LINQ_Assignment_01

{
    internal class Program
    {
        static void Main()
        {
            #region LINQ - Restriction Operators
            // 1. Find all products that are out of stock.
            var OutOfstock = ProductList.Where(P => P.UnitsInStock == 0);
            // 2. Find all products that are in stock and cost more than 3.00 per unit.

            var InStock = ProductList.Where(p => p.UnitsInStock > 0)
                .Where(p => p.UnitPrice > 3.00M);

            //Returns digits whose name is shorter than their value.
            string[] Arr = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var result = Arr.Where((N, I) => N.Length < I);

            #endregion


            #region LINQ - Element Operators
            //Get first Product out of Stock 
            var FirstElement = ProductList.Where(P => P.UnitsInStock == 0).First();
            Console.WriteLine("Fitst Element : " + FirstElement);

            //2. Return the first product whose Price > 1000, unless there is no match, in which case null is returned.

            FirstElement = ProductList.Where(P => P.UnitPrice > 1000).FirstOrDefault();
            Console.WriteLine("Fitst Element  whose Price > 1000 : " + (FirstElement?.ToString() ?? "Not found"));

            // 3.Retrieve the second number greater than 5
            int[] Arr2 = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            var SecondNumber = Arr2.Where(N => N > 5).Skip(1).FirstOrDefault();
            Console.WriteLine($"the second number greater than 5 : {SecondNumber}");
            #endregion
            #region LINQ - Aggregate Operators
            // 1.Uses Count to get the number of odd numbers in the array
            int[] Arr3 = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            var NumberOfOdd = Arr3.Count(n => n % 2 != 0);

            Console.WriteLine("the number of odd numbers in the array:" + NumberOfOdd);


            //2. Return a list of customers and how many orders each has.

            var customers = from cu in CustomerList
                            select new
                            {
                                CustomerName = cu.CustomerName,
                                OrderCount = cu.Orders.Count()
                            };

            foreach (var customer in customers)
            {
                Console.WriteLine($"CustomerName : {customer.CustomerName} ,OrderCount : {customer.OrderCount}  ");
            }

            //3. Return a list of categories and how many products each has

            var categories = from p in ProductList
                             group p by p.Category

                             into CategoryGroup
                             select new
                             {
                                 Category = CategoryGroup.Key,
                                 TotalUnitsInStock = CategoryGroup.Sum(p => p.UnitsInStock)

                             };



            Console.WriteLine("\n =====");
            foreach (var p in categories)
            {
                Console.WriteLine(p);

            }


            //4. Get the total of the numbers in an array.
            int[] Arr4 = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            var total = Arr4.Sum();
            Console.WriteLine("Total sunm = " + total);


            //5. Get the total number of characters of all words in dictionary_english.txt
            //(Read dictionary_english.txt into Array of String First).

            /*Step 1: Read the File:
            System.IO.File.ReadAllLines("dictionary_english.txt") reads all lines from the text file and returns an array of strings. Each line in the file becomes an element in the array.
            If each line represents a word, this is exactly what you want.*/

            string[] dictionary_english = File.ReadAllLines("dictionary_english.txt");

            int totalNumberOfCharacters = dictionary_english.Sum(word => word.Length); //3494688

            Console.WriteLine($"total number of characters of all words  : {totalNumberOfCharacters}");


            //6. Get the length of the shortest word in dictionary_english.txt
            //(Read dictionary_english.txt into Array of String First).
            int shortestWordLenghth = dictionary_english.Min(word => word.Length);
            Console.WriteLine($"the length of the shortest word = {shortestWordLenghth} ");


            //7.Get the length of the longest word in dictionary_english.txt
            //(Read dictionary_english.txt into Array of String First).

            int LongestWordLenghth = dictionary_english.Max(word => word.Length);
            Console.WriteLine($"the length of the longest word = {LongestWordLenghth}");

            //8. Get the average length of the words in dictionary_english.txt
            //(Read dictionary_english.txt into Array of String First).
            var averageLength = dictionary_english.Average(word => word.Length);
            Console.WriteLine($"the average length of the words = {averageLength}");
            //9. Get the total units in stock for each product category.

            var totalUnitsInStock = ProductList.Where(p => p.UnitsInStock > 0)
                                    .GroupBy(p => p.Category)
                                    .Select(x => new
                                    {
                                        CategoryName = x.Key,
                                        totalUnitsInStock = x.Sum(p => p.UnitsInStock)

                                    });
            Console.WriteLine("\n=====================================================");
            foreach (var unit in totalUnitsInStock)
            {
                Console.WriteLine($"CategoryName : {unit.CategoryName} , total Units In Stock = {unit.totalUnitsInStock} ");
            }
            Console.WriteLine("\n=====================================================");

            //10. Get the cheapest price among each category's products

            var CheapestPrice = from p in ProductList
                                group p by p.Category
                                into CategoryGroup
                                select new
                                {
                                    CategoryName = CategoryGroup.Key,
                                    CheapestPrice = CategoryGroup.Min(p => p.UnitPrice)
                                };
            Console.WriteLine("\n=====================================================");
            foreach (var unit in CheapestPrice)
            {
                Console.WriteLine($"CategoryName : {unit.CategoryName} , CheapestPrice = {unit.CheapestPrice} ");
            }
            Console.WriteLine("\n=====================================================");


            //11. Get the products with the cheapest price in each category (Use Let)
            var CheapestProducts = from p in ProductList
                                   group p by p.Category
                                 into CategoryGroup
                                   let cheapestPrice = CategoryGroup.Min(p => p.UnitPrice)

                                   select new
                                   {
                                       CategoryName = CategoryGroup.Key,
                                       CheapestProducts = CategoryGroup.Where(p => p.UnitPrice == cheapestPrice).ToList()
                                   };

            foreach (var category in CheapestProducts)
            {
                Console.WriteLine($"Category name ={category.CategoryName} ");
                foreach (var product in category.CheapestProducts)
                {
                    Console.WriteLine(product);

                }

            }

            // 12. Get the most expensive price among each category's products.
            var expensivePrice = from p in ProductList
                                 group p by p.Category
                                into CategoryGroup
                                 select new
                                 {
                                     CategoryName = CategoryGroup.Key,
                                     expensivePrice = CategoryGroup.Max(p => p.UnitPrice)
                                 };
            Console.WriteLine("\n=====================================================");
            foreach (var unit in expensivePrice)
            {
                Console.WriteLine($"CategoryName : {unit.CategoryName} , expensivePrice = {unit.expensivePrice} ");
            }
            Console.WriteLine("\n=====================================================");

            //13. Get the products with the most expensive price in each category.
            var expensiveProducts = from p in ProductList
                                    group p by p.Category
                                    into CategoryGroup
                                    let expensivePrice0 = CategoryGroup.Max(p => p.UnitPrice)

                                    select new
                                    {
                                        CategoryName = CategoryGroup.Key,
                                        expensiveProducts = CategoryGroup.Where(p => p.UnitPrice == expensivePrice0).ToList()
                                    };

            foreach (var category in expensiveProducts)
            {
                Console.WriteLine($"Category name ={category.CategoryName} ");
                foreach (var product in category.expensiveProducts)
                {
                    Console.WriteLine(product);

                }

            }

            //14. Get the average price of each category's products.
            var AveragePrice = ProductList.GroupBy(p => p.Category)
                               .Select(x => new
                               {
                                   categoryName = x.Key,
                                   AveragePrice = x.Average(p => p.UnitPrice)
                               });

            foreach (var category in AveragePrice)
            {
                Console.WriteLine($"categoryName : {category.categoryName} , AveragePrice  : {category.AveragePrice}");
            }

            #endregion
            #region LINQ - Ordering Operators
            //Sort a list of products by name
            var productsByName = ProductList.OrderBy(p => p.ProductName);
            //foreach (var product in productsByName) {

            //    Console.WriteLine(product);
            //}
            //2. Uses a custom comparer to do a case-insensitive sort of the words in an array.

            //Step 2: Using Array.Sort with a Custom Comparer
            string[] Arr5 = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };
            //Array.Sort is a static method provided by the Array class in C#. It is used to sort the elements
            //of an array in place, meaning the original array is modified to reflect the sorted order.

            //StringComparer.OrdinalIgnoreCase is a predefined comparer in C# that performs a case-insensitive comparison of strings.
            Array.Sort(Arr5, StringComparer.OrdinalIgnoreCase);

            foreach (var word in Arr5)
            {
                Console.Write($"{word} ");
            }
            // 3. Sort a list of products by units in stock from highest to lowest.
            ProductList.Sort();
            //foreach (var product in ProductList)
            //{
            //    Console.WriteLine(product);
            //}

            //4. Sort a list of digits, first by length of their name, and then alphabetically by the name itself.
            string[] Arr6 = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            var ListOFDigits = Arr6.OrderBy(word => word.Length).ThenBy(word => word);
            Console.WriteLine("\n=====================================================");
            foreach (var word in ListOFDigits)
            {
                Console.Write(word + " ");
            }

            Console.WriteLine("\n=====================================================");
            //5. Sort first by-word length and then by a case-insensitive sort of the words in an array.
            string[] Arr7 = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };

            var Words = Arr6.OrderBy(word => word.Length).ThenBy(word => word, StringComparer.OrdinalIgnoreCase);
            //6. Sort a list of products, first by category, and then by unit price, from highest to lowest.
            var ProductSorted = from p in ProductList
                                orderby p.Category, p.UnitPrice descending
                                select p;

            //7. Sort first by-word length and then by a case-insensitive descending sort of the words in an array.
            Words = Arr6.OrderBy(word => word.Length).ThenByDescending(word => word, StringComparer.OrdinalIgnoreCase);
            //8. Create a list of all digits in the array whose second letter is 'i'
            // that is reversed from the order in the original array.
            var Reversed = Arr6.Where((c, I) => c[1] == 'i').Reverse();
            foreach (var word in Reversed)
            {
                Console.Write(word + " ***");
            }

            #region Transformation Operators
            //1. Return a sequence of just the names of a list of products.

            var ProductsNames = ProductList.Select(p => p.ProductName);
            //foreach (var product in ProductsNames)
            //{
            //    Console.WriteLine(product);
            //}


            //2. Produce a sequence of the uppercase and lowercase versions of each word in the original array
            //(Anonymous Types).
            string[] words = { "aPPLE", "BlUeBeRrY", "cHeRry" };

            var cases = words.Select(word => new
            {
                UpperCase = word.ToUpper(),
                LowerCase = word.ToLower(),
            });

            foreach (var word in cases)
            {
                Console.WriteLine($"UpperCase : {word.UpperCase} , LowerCase :{word.LowerCase}");

            }

            //3. Produce a sequence containing some properties of Products,
            //including UnitPrice which is renamed to Price in the resulting type.
            var sequence = ProductList.Select(p => new
            {
                Price = p.UnitPrice,
            });


            //4. Determine if the value of int in an array match their position in the array.
            int[] Arr8 = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var MatchPoition = Arr8.Select((number, Index) => new
            {
                value = number,
                Index = Index,
                IsinPlace = number == Index

            });

            Console.WriteLine("Number In place ?");
            foreach (var number in MatchPoition)
            {
                Console.WriteLine($"{number.value} : {number.IsinPlace}");


            }


            //5. Returns all pairs of numbers from both arrays such that the number from numbersA is less than the number from numbersB.
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };

            for (int i = 0; i < numbersA.Length; i++)
            {
                {
                    for (int j = 0; j < numbersB.Length; j++)
                    {
                        if (numbersA[i] < numbersB[j])
                            Console.WriteLine($"{numbersA[i]} is less than {numbersB[j]}");

                    }
                }

                var ordersFrom1998OrLater = from c in CustomerList
                                            from o in c.Orders
                                            where o.OrderDate.Year >= 1998
                                            select o;
                //6. Select all orders where the order total is less than 500.00.
                var orders0 = from c in CustomerList
                             from o in c.Orders
                             where o.Total < 500.00M
                             select o 
                             ;
               foreach (var o in orders0)
                {
                    Console.WriteLine(o);
                }
                //7. Select all orders where the order was made in 1998 or later.

                var OrderDate = CustomerList.SelectMany(c => c.Orders).Where(o => o.OrderDate.Year > 1998);
                //foreach(var order in OrderDate)
                //{
                //    Console.WriteLine(order);
                //}
                #region LINQ - Set Operators
                //Distinct, Union, Intersect, and Except
                // 1.Find the unique Category names from Product List

                var uniqueCategory = ProductList.Distinct();
                // foreach (var u in uniqueCategory) Console.WriteLine(u) ;


                //2.Produce a Sequence containing the unique first letter from both product and customer names.

                //// Extract the first letter from product names

                var ProductFirstLetter = ProductList.Select(p => p.ProductName[0]);
                //// Extract the first letter from Customer names
                var CustomerFirstLetter = CustomerList.Select(c => c.CustomerName[0]);

                var Sequence = ProductFirstLetter.Union(CustomerFirstLetter);


                //3.Create one sequence that contains the common first letter from both product and customer names.

                 Sequence = ProductFirstLetter.Intersect(CustomerFirstLetter);

                //4.Create one sequence that contains the first letters of product names that are not also first letters of customer names.
                Sequence = ProductFirstLetter.Except(CustomerFirstLetter);
                foreach(var c in Sequence) Console.Write(c + " ");


                //5.Create one sequence that contains the last Three Characters
                //in each name of all customers and products, including any duplicates

                var lastThreeCharactersProducts = ProductList.Select(p => p.ProductName.Length >= 3 ? p.ProductName.Substring(p.ProductName.Length - 3) : p.ProductName);
                var LastThreeCharactersCustomers = CustomerList.Select(c => c.CustomerName.Length >= 3 ? c.CustomerName.Substring(c.CustomerName.Length - 3) : c.CustomerName);
                var Sequence02 = lastThreeCharactersProducts.Concat(LastThreeCharactersCustomers);
                #endregion

                #region LINQ - Partitioning Operators
                /*Take
                    TakeWhile
                    Skip
                    SkipWhile*/

                // 1.Get the first 3 orders from customers in Washington
                var orders = CustomerList.Where(c => c.City == "Washington").SelectMany(c => c.Orders) .Take(3);
                //2.Get all but the first 2 orders from customers in Washington
                var allButFirstTwoOrders = CustomerList
                     .Where(c => c.City == "Washington")
                     .SelectMany(c => c.Orders)   
                     .Skip(2);

                //3. Return elements starting from the beginning of the array
                //until a number is hit that is less than its position in the array.
                int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
                var result2 = numbers.TakeWhile((n, I) => n > I);
                foreach (var order in result2)
                    Console.WriteLine(order);

                Console.WriteLine("\n===========");

                //4.Get the elements of the array starting from the first element divisible by 3.
                result2 = numbers.SkipWhile((n => n % 3 != 0));
                foreach (var order in result2)
                    Console.Write(order + " ");
                //5.Get the elements of the array starting from the first element less than its position.
                Console.WriteLine("\n===========");
                result2 = numbers.SkipWhile((n, I) => n > I);
                foreach (var order in result2)
                    Console.Write(order + " ");

                #endregion


                #region LINQ - Quantifiers

                /*
                 Any
                All
                Contains*/
                 //1.Determine if any of the words in dictionary_english.txt
                //(Read dictionary_english.txt
                //into Array of String First) contain the substring 'ei'.
                dictionary_english = File.ReadAllLines("dictionary_english.txt");
                bool substringIE = dictionary_english.Contains("ei");
                Console.WriteLine(substringIE);

                //2.Return a grouped a list of products only for
                //categories that have at least one product that is out of stock.
                var result5 = ProductList.Any(p=> p.UnitsInStock == 0);
                //3.Return a grouped a list of products only for categories that have all of their products in stock.
                var result6 = ProductList.Any(p=> p.UnitsInStock > 0);
                //
                #endregion
                #region LINQ – Grouping Operators
                //1.	Use group by to partition a list of numbers by their remainder when divided by 5
                List<int> numbers2 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
               var groupList = numbers2.GroupBy(n => n % 5 );
                Console.WriteLine("============");

                foreach ( var group in groupList)
                {
                    Console.WriteLine($"Numbers with remainder {group.Key} when divided by 5:");
                    foreach (var n in group)
                    {
                        Console.WriteLine(" "  +n);
                    }
                }
                //2.2.	Uses group by to partition a list of words by their first letter.
                // Use dictionary_english.txt for Input
                var GroupByFirstLeter = dictionary_english.GroupBy(word => word[0]);

               


                //3.Consider this Array as an Input
                //Use Group By with a custom comparer that matches words that are consists of the same Characters Together

                string[] Arr011= { "from", "salt", "earn", " last", "near", "form"};

               



                #endregion





                #endregion



                #endregion









            }
        }
    }
}


