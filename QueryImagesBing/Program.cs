/***************************
Written by Michelle J. Salvador
Using many tutorials provided by Microsoft to get it to work...

Use this to query Bing using a csv file with queries in  the first column.
Make sure that the first row already has the querys. No titles.
Make sure that the first column has the query id for each query.
Check the example .csv file [6100_Spanish_contempt.csv] for example formatting.
User must have a Microsoft Azure key to query the marketplace. 

Provide the file path that query results will be stored into. 
Create files as csv before hand.
Results will be provided as urls of the nth (I think I have it set to 15 pages right now) Bing image page for each query in the query file.
***************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net; //needed for authentication
using QueryImagesBing.BingSearch;//using ApplicationName. ServiceRefrenceName

namespace QueryImagesBing
{
    
     static class Program
    {
        static void Main(string[] args)
        {
            var readFrom = @"C:\Users\Chemil5000v2\Desktop\queries\6100_Spanish_contempt.csv";
            String writeTo = @"C:\Users\Chemil5000v2\Desktop\queries\Bing_6100_spanish_contempt.csv";
            string[] csvRows = System.IO.File.ReadAllLines(readFrom);
            var csv = new StringBuilder();
            try
            { 
                using (StreamReader sr = new StreamReader(readFrom))
               {
                string currentLine;
                    while((currentLine = sr.ReadLine()) !=null)
                    {
                        
                        csvRows = currentLine.Split(',');
                        var queryId = int.Parse(csvRows[0]);
                        var queryPlus= csvRows[1].ToString();
                        //queryNeg for taking away query items you do not want returned. Concatenate with queryPlus when making the request.
                        // var queryNeg = " -birthday -clipart -cartoon -anime -drawing -\"clip+art\" -\"animated+gif\"";
                        MakeRequest(queryPlus, 15, queryId, csv);
                        
                    }
                    File.WriteAllText(writeTo, csv.ToString()); 

                }          
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed and Catched!!!");
                File.WriteAllText(writeTo, csv.ToString()); 
                string innerMessage =
                    (ex.InnerException != null) ? ex.InnerException.Message : String.Empty;
                Console.WriteLine("{0}\n{1}", ex.Message, innerMessage);
            }
        }

        static void MakeRequest(string query, int pages, int queryId,  StringBuilder csv)
        {
                        
            //There was an issue here before were I was refrencing BingSearch.Bing instead...
            var bingContainer = new Bing.BingSearchContainer(
              new Uri("https://api.datamarket.azure.com/Bing/Search/"));
 
            //Azure Bing Marketplace account key (Replace with user key as needed)
            var accountKey =
                "ZAdrCA5ePnFhop/B5qhOxTDEwJ106mrxq6Sx4yJ/QBI"; //Dr. Mahoors
              //  "iisMD4XczRIebB8lTU+ZqDwkYcV6BM/HRH4OFbLFlxk"; //hojjat


            bingContainer.Credentials = new NetworkCredential(accountKey, accountKey);
 
            //do the query
            int skipNum;
                String firstURL = null;
                String secondURL = null;
                String thirdURL = null;
                Boolean firstTrue = false;
                Boolean secondTrue = false;
                Boolean thirdTrue = false; 
            for (int pagesIndex = 1; pagesIndex <= pages; pagesIndex++ )
            {
                skipNum = ((pagesIndex - 1) * 50);//this is from what image to start from
                //query takes query(happy boy face), market(), adult, latitude, longitude, web file type(Face:Face).
                var imageQuery = bingContainer.Image(query, null, null, null, null, null, "Face:Face");
               //  Console.WriteLine(query);
                imageQuery = imageQuery.AddQueryOption("$skip", skipNum);

                Console.WriteLine(skipNum+" skipNum");
                Console.WriteLine(pagesIndex+" pagesIndex");

                //for some reason I'm getting forced off and adding waits help....
                System.Threading.Thread.Sleep(200);
                var imageResults = imageQuery.Execute(); //the magic line       


                bool moreImages = false;
                int imageCount = 0;

                foreach (var result in imageResults)
                {
                    Console.WriteLine(result.MediaUrl);
                    var newLine = string.Format("\"{0}\", \"Bing\", \"{1}\", \"{2}\"", result.MediaUrl, queryId, result.Title);
                    Console.WriteLine("{0},{1}", queryId, query);
                    csv.AppendLine(newLine);
                    moreImages = true;
                    imageCount++;
                    
                    Console.WriteLine(imageCount + " imageCount");
                    if (imageCount == 1)
                    {
                        if (firstURL == result.MediaUrl)
                        { firstTrue = true; }
                        firstURL = result.MediaUrl;
                        Console.WriteLine("firstURL");
                    }
                    if (imageCount == 2)
                    {
                        if (secondURL == result.MediaUrl)
                        { secondTrue = true; }
                        secondURL = result.MediaUrl;
                        Console.WriteLine("secondURL");
                    }
                    if (imageCount == 3)
                    {
                        if (thirdURL == result.MediaUrl)
                        { thirdTrue = true; }
                        thirdURL = result.MediaUrl;
                        Console.WriteLine("thirdURL");
                    }

                    if (firstTrue && secondTrue && thirdTrue)
                    {
                        Console.WriteLine("NO MORE IMAGES REPEATS! //////" + query + "," + imageCount + ", pages " + pagesIndex);
                        firstURL = null;
                        secondURL = null;
                        thirdURL = null;
                        firstTrue = false;
                        secondTrue = false;
                        thirdTrue = false;
                        //Console.ReadLine();
                        return;
                    }
                    
                }

                if (!moreImages || imageCount < 50 )
                {
                    
                    Console.WriteLine("NO MORE IMAGES! //////" +query+","+ imageCount +", pages "+pagesIndex);
                firstURL = null;
                secondURL = null;
                firstTrue = false;
                secondTrue = false;
                   // Console.ReadLine();
                    return;
                }
                

            }          
              
        }
    }
}

