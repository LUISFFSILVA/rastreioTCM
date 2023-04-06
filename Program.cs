using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Threading.Tasks;

// Criar um objeto HttpClient
HttpClient client = new HttpClient();

// Definir o nome de usuário e a senha
string username = "admin@email.com";
string password = "Rastr@Scania#2019";
string? token;

// Criar o objeto que irá armazenar as credenciais
var credentials = new Dictionary<string, string>
{
    { "email", username },
    { "password", password }
};

// Serializar as credenciais em formato JSON
string json = JsonSerializer.Serialize(credentials);

// Enviar a requisição e receber a resposta
HttpResponseMessage response = await client.PostAsync("http://rastreamento.tcm.com.br/auth/login", new StringContent(json, Encoding.UTF8, "application/json"));
// Verificar se a resposta foi bem sucedida
if (response.IsSuccessStatusCode)
{
    // Ler o conteúdo da resposta como uma string
    string responseBody = await response.Content.ReadAsStringAsync();
    // Desserializar a string em um objeto
    var obj = JsonSerializer.Deserialize<tokenRecebido>(responseBody);
    // Processar a resposta
    Console.WriteLine(obj?.token);
    token = obj?.token;
    var position = await getPosition(token);
    var pos = JsonSerializer.Deserialize<dadosVehicle>(position);
    Console.WriteLine(pos);
}
else if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
{
    // Tratar o erro de acordo com a lógica de negócio
    string responseBody = await response.Content.ReadAsStringAsync();
    Console.WriteLine("Erro 422: " + responseBody);
}
else
{
    Console.WriteLine(response);
    // Tratar o erro
}

static async Task<string> getPosition(string tokenTCM)
{
    var url = "http://rastreamento.tcm.com.br/last-position/ECV9005";
    var apiKey = tokenTCM;
    var httpClient = new HttpClient();

    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

    var response = await httpClient.GetAsync(url);

    if (response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);
        return responseBody;
    }
    else
    {
        Console.WriteLine($"Erro na solicitação: {response.StatusCode} - {response.ReasonPhrase}");
        return null;
    }
}

public class tokenRecebido
{
    public string? token { get; set; }
}

public class dadosVehicle
{
    public string placa { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public string data_hora { get; set; }
}
