<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<GeoCoding.Address>>" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Geocoding Sample
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <form action="<%= Url.Action("geocode", "home") %>" method="get">
        <h2>Geocoding Sample</h2>
        <p>
            <label for="address">Address:</label>
            <%= Html.TextBox("address") %>
            <input type="submit" value="Geocode" />
        </p>
    </form>
    
    <% if (Model != null && Model.Any()) { %>
        <ul>
        <% foreach (var address in Model) { %>
            <li>
                <%= address.Street %><br />
                <%= address.City %>, <%= address.State %> <%= address.PostalCode %>, <%= address.Country %><br />
                (<%= address.Coordinates.Latitude %>, <%= address.Coordinates.Longitude %>)<br />
                <%= address.Accuracy %>
            </li>
        <% } %>
        </ul>
    <% } %>
</asp:Content>