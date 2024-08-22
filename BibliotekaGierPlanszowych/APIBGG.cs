using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows;
using System.Collections;
using RestSharp;
using System.IO;

namespace BibliotekaGierPlanszowych
{
    class APIBGG
    {
        
        public List<string> SearchBoardGame(string gameName)
        {
            List<string> resultsList = new List<string>();
            var client = new RestClient("https://boardgamegeek.com/xmlapi/");
            var request = new RestRequest("search", Method.Get);
            request.AddParameter("search", gameName);

            try
            {
                var response = client.Execute(request);
                if (response.IsSuccessful && response.Content != null)
                {
                    SearchResults searchResults = DeserializeXml<SearchResults>(response.Content);
                    resultsList = DisplayResults(searchResults);
                }
                else
                {
                    MessageBox.Show($"Błąd w zapytaniu: {response.StatusCode}", "Błąd");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd");
            }
            return resultsList;
        }

        private SearchResults DeserializeXml<SearchResults>(string xmlContent)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SearchResults));
            using (StringReader reader = new StringReader(xmlContent))
            {
                return (SearchResults)serializer.Deserialize(reader);
            }
        }

        private List<string> DisplayResults(SearchResults searchResults)
        {
            List<string> resultsListBox = new List<string>();
            resultsListBox.Clear();
            foreach (var game in searchResults.BoardGames)
            {
                resultsListBox.Add($"ID: {game.Id}, Nazwa: {game.Name.Value}, Rok: {game.YearPublished}");
            }
            return resultsListBox;
        }
    }

    [XmlRoot("boardgames")]
    public class SearchResults
    {
        [XmlElement("boardgame")]
        public List<BoardGame> BoardGames { get; set; }
    }

    public class BoardGame
    {
        [XmlAttribute("objectid")]
        public int Id { get; set; }

        [XmlElement("name")]
        public BoardGameName Name { get; set; }

        [XmlElement("yearpublished")]
        public int YearPublished { get; set; }
    }

    public class BoardGameName
    {
        [XmlAttribute("primary")]
        public bool IsPrimary { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
