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
using System.Xml.Linq;

namespace BibliotekaGierPlanszowych
{
    class APIBGG
    {
        private const string BGG_API_URL = "https://boardgamegeek.com/xmlapi/collection/";

        public List<BoardGame> SearchBoardGame(string gameName)
        {
            List<BoardGame> resultsList = new List<BoardGame>();
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

        public List<BoardGameBgg> AddBoardGame(int bggId)
        {
            List<BoardGameBgg> boardGameBgg = new List<BoardGameBgg>();
                        
            var client = new RestClient("https://boardgamegeek.com/xmlapi/");
            var request = new RestRequest($"boardgame/{bggId}", Method.Get);

            try
            {
                var response = client.Execute(request);
                if (response.IsSuccessful && response.Content != null)
                {
                    SearchGameResults searchGameResults = DeserializeXml<SearchGameResults>(response.Content);
                    boardGameBgg = DisplayGameResults(searchGameResults);
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

            return boardGameBgg;
        }

        private SearchResults DeserializeXml<SearchResults>(string xmlContent)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SearchResults));
            using (StringReader reader = new StringReader(xmlContent))
            {
                return (SearchResults)serializer.Deserialize(reader);
            }
        }

        private List<BoardGame> DisplayResults(SearchResults searchResults)
        {
            List<BoardGame> resultsListBox = new List<BoardGame>();
            resultsListBox.Clear();
            foreach (var game in searchResults.Items)
            {
                resultsListBox.Add(game);
            }
            return resultsListBox;
        }

        private SearchGameResults DeserializeGameXml<SearchGameResults>(string xmlContent)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SearchGameResults));
            using (StringReader reader = new StringReader(xmlContent))
            {
                return (SearchGameResults)serializer.Deserialize(reader);
            }
        }

        private List<BoardGameBgg> DisplayGameResults(SearchGameResults searchGameResults)
        {
            List<BoardGameBgg> resultsGameListBox = new List<BoardGameBgg>();
            resultsGameListBox.Clear();
            foreach (var game in searchGameResults.Items)
            {
                resultsGameListBox.Add(game);
            }
            return resultsGameListBox;
        }

        private SearchCollectionResults DeserializeCollectionXml<SearchCollectionResults>(string xmlContent)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SearchCollectionResults));
            using (StringReader reader = new StringReader(xmlContent))
            {
                return (SearchCollectionResults)serializer.Deserialize(reader);
            }
        }

        private List<Item> DisplayCollectionResults(Items items)
        {
            List<Item> resultCollectionListBox = new List<Item>();
            resultCollectionListBox.Clear();
            foreach (var game in items.ItemList)
            {
                resultCollectionListBox.Add(game);
            }
            return resultCollectionListBox;
        }

        public List<Item> GetBoardGamesForUser(string username)
        {
            List<Item> collection = new List<Item>();

            var client = new RestClient("https://boardgamegeek.com/xmlapi/");
            var request = new RestRequest($"collection/{username}", Method.Get);

            try
            {
                var response = client.Execute(request);
                if (response.IsSuccessful && response.Content != null)
                {
                    //Items searchCollectionResults = DeserializeXml<Items>(response.Content);
                    //collection = DisplayCollectionResults(searchCollectionResults);
                    var serializer = new XmlSerializer(typeof(Items));
                    using (StringReader reader = new StringReader(response.Content))
                    {
                        
                        Items items = (Items)serializer.Deserialize(reader);
                        collection = items?.ItemList;
                        
                    }
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

            return collection;
        }

    }

    // Klasa reprezentująca wyniki wyszukiwania
    [XmlRoot("boardgames")]
    public class SearchResults
    {
        [XmlElement("boardgame")]
        public List<BoardGame> Items { get; set; }
    }

    public class BoardGame
    {

        [XmlElement("name")]
        public string Nazwa { get; set; }       

        [XmlElement("yearpublished")]
        public string RokPublikacji { get; set; }

        [XmlAttribute("objectid")]
        public int Id { get; set; }
    }

    [XmlRoot("boardgames")]
    public class SearchGameResults
    {
        [XmlElement("boardgame")]
        public List<BoardGameBgg> Items { get; set; }
    }

    [XmlRoot("boardgame")]
    public class BoardGameBgg
    {
    [XmlAttribute("objectid")]
        public int ObjectId { get; set; }

        [XmlElement("yearpublished")]
        public int YearPublished { get; set; }

        [XmlElement("minplayers")]
        public int MinPlayers { get; set; }

        [XmlElement("maxplayers")]
        public int MaxPlayers { get; set; }

        [XmlElement("playingtime")]
        public int PlayingTime { get; set; }

        [XmlElement("minplaytime")]
        public int MinPlayTime { get; set; }

        [XmlElement("maxplaytime")]
        public int MaxPlayTime { get; set; }

        [XmlElement("age")]
        public int Age { get; set; }

        [XmlElement("thumbnail")]
        public string image_url{ get; set; }

        [XmlElement("name")]
        public List<Name> Name { get; set; }
    }
   
    public class Name
    {
        [XmlAttribute("primary")]
        public bool IsPrimary { get; set; }

        [XmlAttribute("sortindex")]
        public int SortIndex { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    //Kolekcja z BGG
    [XmlRoot("items")]
    public class Items
    {
        [XmlAttribute("totalitems")]
        public int TotalItems { get; set; }

        [XmlElement("item")]
        public List<Item> ItemList { get; set; }
    }

    public class Item
    {
        [XmlAttribute("objecttype")]
        public string ObjectType { get; set; }

        [XmlAttribute("objectid")]
        public int ObjectId { get; set; }

        [XmlAttribute("subtype")]
        public string Subtype { get; set; }

        [XmlAttribute("collid")]
        public long CollId { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("yearpublished")]
        public int YearPublished { get; set; }

        [XmlElement("image")]
        public string Image { get; set; }

        [XmlElement("thumbnail")]
        public string Thumbnail { get; set; }

        [XmlElement("stats")]
        public Stats stats { get; set; }

    }

    public class Stats
    {
        [XmlAttribute("minplayers")]
        public int MinPlayers { get; set; }

        [XmlAttribute("maxplayers")]
        public int MaxPlayers { get; set; }
     
        [XmlAttribute("playingtime")]
        public int PlayingTime { get; set; }

        [XmlElement("rating")]
        public Rating Rating { get; set; }
    }

    public class Rating
    {
        [XmlAttribute("value")]
        public string  Value { get; set; }

    }

    public class UsersRated
    {
        [XmlAttribute("value")]
        public int Value { get; set; }
    }    

}
