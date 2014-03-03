namespace IndexAndSearchFileStore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Lucene.Net.Index;
    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Search;
    using Lucene.Net.Store;
    using Lucene.Net.QueryParsers;

    internal class IndexHelpers
    {
        internal static void CreateOrAppendIndex(string indexFolderPath, string toIndexFolderPath, bool verbose)
        {
            IndexHelpers.UpdateIndex(ParseFolderTree(new System.IO.DirectoryInfo(toIndexFolderPath), verbose), indexFolderPath);
        }

        private static void UpdateIndex(List<string> folderParseResults, string indexFolderPath)
        {
            Console.WriteLine(" > Adding items to index ...");

            FSDirectory directory = null;
            Analyzer analyzer = null;
            IndexWriter indexWriter = null;

            try
            {
                directory = FSDirectory.Open(new System.IO.DirectoryInfo(indexFolderPath));
                analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                indexWriter = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("NOTE!", ex);
            }

            foreach (string item in folderParseResults)
            {
                Document document = new Document();
                document.Add(new Field("FilePathString", item, Field.Store.YES, Field.Index.ANALYZED));
                indexWriter.AddDocument(document);
            }

            indexWriter.Optimize();
            indexWriter.Dispose();
            analyzer.Dispose();
            directory.Dispose();
        }

        private static List<string> ParseFolderTree(DirectoryInfo parentFolder, bool verbose)
        {
            if (verbose) Console.WriteLine(" > Parsing folder " + parentFolder.FullName + " ...");

            List<String> listToReturn = new List<string>();
            FileInfo[] files = null;
            DirectoryInfo[] subFolders = null;

            // First, process all the files directly under this folder 
            try
            {
                files = parentFolder.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch  (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (PathTooLongException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (FileInfo file in files)
                {
                    listToReturn.Add(file.FullName);
                }

                // parse all sub folders
                subFolders = parentFolder.GetDirectories();

                foreach (DirectoryInfo subFolder in subFolders)
                {
                    try
                    {
                        listToReturn.AddRange(ParseFolderTree(subFolder, verbose));
                    }
                    catch (PathTooLongException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
            }

            return listToReturn;
        }

        internal static void SearchIndex(string indexFolderPath, string searchTerm)
        {
            IndexReader reader;
            Analyzer analyzer;
            Searcher searcher;

            try
            {
                FSDirectory directory = FSDirectory.Open(new System.IO.DirectoryInfo(indexFolderPath));
                reader = IndexReader.Open(directory, true);
                analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                searcher = new IndexSearcher(reader);
                QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "FilePathString", analyzer);
                Query query = parser.Parse(searchTerm);

                TopScoreDocCollector collector = TopScoreDocCollector.Create(100, true);
                searcher.Search(query, collector);
                ScoreDoc[] hits = collector.TopDocs().ScoreDocs;

                foreach (ScoreDoc scoreDoc in hits)
                {
                    Document document = searcher.Doc(scoreDoc.Doc);
                    Console.WriteLine(document.Get("FilePathString"));
                }

                reader.Dispose();
                searcher.Dispose();
                analyzer.Dispose();
            }
            catch (ParseException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("NOTE!", ex);
            }
            finally
            {
            }
        }
    }
}
