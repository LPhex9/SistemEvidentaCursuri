using Microsoft.Data.SqlClient;
using SistemEvidentaCursuri.Models;

namespace SistemEvidentaCursuri.Data
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString =
            @"Server=.\SQLEXPRESS03;Database=CentruInstruire;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // ─── CURSANȚI ───────────────────────────────────────────────────────

        public static List<Cursant> GetAllCursanti()
        {
            var list = new List<Cursant>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM Cursant ORDER BY Nume", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Cursant
                {
                    IdCursant = (int)reader["IdCursant"],
                    Nume      = reader["Nume"].ToString()!,
                    Prenume   = reader["Prenume"].ToString()!,
                    Telefon   = reader["Telefon"].ToString()!,
                    Email     = reader["Email"].ToString()!
                });
            }
            return list;
        }

        public static List<Cursant> SearchCursanti(string text)
        {
            var list = new List<Cursant>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT * FROM Cursant WHERE Nume LIKE @t OR Prenume LIKE @t OR Email LIKE @t ORDER BY Nume", conn);
            cmd.Parameters.AddWithValue("@t", $"%{text}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Cursant
                {
                    IdCursant = (int)reader["IdCursant"],
                    Nume      = reader["Nume"].ToString()!,
                    Prenume   = reader["Prenume"].ToString()!,
                    Telefon   = reader["Telefon"].ToString()!,
                    Email     = reader["Email"].ToString()!
                });
            }
            return list;
        }

        public static void AddCursant(Cursant c)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO Cursant (Nume, Prenume, Telefon, Email) VALUES (@n, @p, @t, @e)", conn);
            cmd.Parameters.AddWithValue("@n", c.Nume);
            cmd.Parameters.AddWithValue("@p", c.Prenume);
            cmd.Parameters.AddWithValue("@t", c.Telefon);
            cmd.Parameters.AddWithValue("@e", c.Email);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateCursant(Cursant c)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE Cursant SET Nume=@n, Prenume=@p, Telefon=@t, Email=@e WHERE IdCursant=@id", conn);
            cmd.Parameters.AddWithValue("@n", c.Nume);
            cmd.Parameters.AddWithValue("@p", c.Prenume);
            cmd.Parameters.AddWithValue("@t", c.Telefon);
            cmd.Parameters.AddWithValue("@e", c.Email);
            cmd.Parameters.AddWithValue("@id", c.IdCursant);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteCursant(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Cursant WHERE IdCursant=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static bool EmailExistsCursant(string email, int excludeId = 0)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Cursant WHERE Email=@e AND IdCursant<>@id", conn);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@id", excludeId);
            return (int)cmd.ExecuteScalar()! > 0;
        }

        public static bool CursantHasInscrieri(int idCursant)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Inscriere WHERE IdCursant=@id", conn);
            cmd.Parameters.AddWithValue("@id", idCursant);
            return (int)cmd.ExecuteScalar()! > 0;
        }

        // ─── CURSURI ─────────────────────────────────────────────────────────

        public static List<Curs> GetAllCursuri()
        {
            var list = new List<Curs>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM Curs ORDER BY Denumire", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Curs
                {
                    IdCurs     = (int)reader["IdCurs"],
                    Denumire   = reader["Denumire"].ToString()!,
                    Formator   = reader["Formator"].ToString()!,
                    Pret       = (decimal)reader["Pret"],
                    DurataZile = (int)reader["DurataZile"]
                });
            }
            return list;
        }

        public static List<Curs> FilterCursuri(string formator, int? durata)
        {
            var list = new List<Curs>();
            using var conn = GetConnection();
            conn.Open();
            var sql = "SELECT * FROM Curs WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(formator)) sql += " AND Formator LIKE @f";
            if (durata.HasValue) sql += " AND DurataZile=@d";
            sql += " ORDER BY Denumire";
            using var cmd = new SqlCommand(sql, conn);
            if (!string.IsNullOrWhiteSpace(formator))
                cmd.Parameters.AddWithValue("@f", $"%{formator}%");
            if (durata.HasValue)
                cmd.Parameters.AddWithValue("@d", durata.Value);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Curs
                {
                    IdCurs     = (int)reader["IdCurs"],
                    Denumire   = reader["Denumire"].ToString()!,
                    Formator   = reader["Formator"].ToString()!,
                    Pret       = (decimal)reader["Pret"],
                    DurataZile = (int)reader["DurataZile"]
                });
            }
            return list;
        }

        public static void AddCurs(Curs c)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO Curs (Denumire, Formator, Pret, DurataZile) VALUES (@d, @f, @p, @dz)", conn);
            cmd.Parameters.AddWithValue("@d", c.Denumire);
            cmd.Parameters.AddWithValue("@f", c.Formator);
            cmd.Parameters.AddWithValue("@p", c.Pret);
            cmd.Parameters.AddWithValue("@dz", c.DurataZile);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateCurs(Curs c)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE Curs SET Denumire=@d, Formator=@f, Pret=@p, DurataZile=@dz WHERE IdCurs=@id", conn);
            cmd.Parameters.AddWithValue("@d", c.Denumire);
            cmd.Parameters.AddWithValue("@f", c.Formator);
            cmd.Parameters.AddWithValue("@p", c.Pret);
            cmd.Parameters.AddWithValue("@dz", c.DurataZile);
            cmd.Parameters.AddWithValue("@id", c.IdCurs);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteCurs(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Curs WHERE IdCurs=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static bool CursHasInscrieri(int idCurs)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Inscriere WHERE IdCurs=@id", conn);
            cmd.Parameters.AddWithValue("@id", idCurs);
            return (int)cmd.ExecuteScalar()! > 0;
        }

        // ─── ÎNSCRIERI ───────────────────────────────────────────────────────

        public static List<Inscriere> GetAllInscrieri()
        {
            var list = new List<Inscriere>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT i.*, c.Nume+' '+c.Prenume AS NumeCursant,
                       cu.Denumire AS DenumireCurs, cu.Pret AS PretCurs
                FROM Inscriere i
                JOIN Cursant c ON i.IdCursant = c.IdCursant
                JOIN Curs cu ON i.IdCurs = cu.IdCurs
                ORDER BY i.DataInscriere DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Inscriere
                {
                    IdInscriere   = (int)reader["IdInscriere"],
                    IdCursant     = (int)reader["IdCursant"],
                    IdCurs        = (int)reader["IdCurs"],
                    DataInscriere = (DateTime)reader["DataInscriere"],
                    StatusPlata   = reader["StatusPlata"].ToString()!,
                    NumeCursant   = reader["NumeCursant"].ToString()!,
                    DenumireCurs  = reader["DenumireCurs"].ToString()!,
                    PretCurs      = (decimal)reader["PretCurs"]
                });
            }
            return list;
        }

        public static List<Inscriere> GetInscrieriByCursant(int idCursant)
        {
            var list = new List<Inscriere>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT i.*, c.Nume+' '+c.Prenume AS NumeCursant,
                       cu.Denumire AS DenumireCurs, cu.Pret AS PretCurs
                FROM Inscriere i
                JOIN Cursant c ON i.IdCursant = c.IdCursant
                JOIN Curs cu ON i.IdCurs = cu.IdCurs
                WHERE i.IdCursant = @id
                ORDER BY i.DataInscriere DESC", conn);
            cmd.Parameters.AddWithValue("@id", idCursant);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Inscriere
                {
                    IdInscriere   = (int)reader["IdInscriere"],
                    IdCursant     = (int)reader["IdCursant"],
                    IdCurs        = (int)reader["IdCurs"],
                    DataInscriere = (DateTime)reader["DataInscriere"],
                    StatusPlata   = reader["StatusPlata"].ToString()!,
                    NumeCursant   = reader["NumeCursant"].ToString()!,
                    DenumireCurs  = reader["DenumireCurs"].ToString()!,
                    PretCurs      = (decimal)reader["PretCurs"]
                });
            }
            return list;
        }

        public static bool InscriereExista(int idCursant, int idCurs)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Inscriere WHERE IdCursant=@c AND IdCurs=@cu", conn);
            cmd.Parameters.AddWithValue("@c", idCursant);
            cmd.Parameters.AddWithValue("@cu", idCurs);
            return (int)cmd.ExecuteScalar()! > 0;
        }

        public static void AddInscriere(Inscriere i)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO Inscriere (IdCursant, IdCurs, DataInscriere, StatusPlata) VALUES (@c, @cu, @d, @s)", conn);
            cmd.Parameters.AddWithValue("@c", i.IdCursant);
            cmd.Parameters.AddWithValue("@cu", i.IdCurs);
            cmd.Parameters.AddWithValue("@d", i.DataInscriere);
            cmd.Parameters.AddWithValue("@s", i.StatusPlata);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteInscriere(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Inscriere WHERE IdInscriere=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // ─── RAPORT ──────────────────────────────────────────────────────────

        public static List<(string NumeComplet, int NrInscrieri, decimal SumaTotala)> GetRaportSumar()
        {
            var list = new List<(string, int, decimal)>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT c.Nume+' '+c.Prenume AS NumeComplet,
                       COUNT(i.IdInscriere) AS NrInscrieri,
                       ISNULL(SUM(CASE WHEN i.StatusPlata='Achitat' THEN cu.Pret ELSE 0 END), 0) AS SumaTotala
                FROM Cursant c
                LEFT JOIN Inscriere i ON c.IdCursant = i.IdCursant
                LEFT JOIN Curs cu ON i.IdCurs = cu.IdCurs
                GROUP BY c.IdCursant, c.Nume, c.Prenume
                ORDER BY SumaTotala DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add((
                    reader["NumeComplet"].ToString()!,
                    (int)reader["NrInscrieri"],
                    (decimal)reader["SumaTotala"]
                ));
            }
            return list;
        }

        public static (int TotalCursanti, decimal SumaTotala, decimal MediePerCursant, string CursTop) GetStatistici()
        {
            using var conn = GetConnection();
            conn.Open();

            int totalCursanti;
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Cursant", conn))
                totalCursanti = (int)cmd.ExecuteScalar()!;

            decimal sumaTotala;
            using (var cmd = new SqlCommand(
                "SELECT ISNULL(SUM(cu.Pret),0) FROM Inscriere i JOIN Curs cu ON i.IdCurs=cu.IdCurs WHERE i.StatusPlata='Achitat'", conn))
                sumaTotala = (decimal)cmd.ExecuteScalar()!;

            decimal medie = totalCursanti > 0 ? sumaTotala / totalCursanti : 0;

            string cursTop = "N/A";
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 cu.Denumire FROM Curs cu
                JOIN Inscriere i ON cu.IdCurs=i.IdCurs
                GROUP BY cu.IdCurs, cu.Denumire
                ORDER BY COUNT(*) DESC", conn))
            {
                var val = cmd.ExecuteScalar();
                if (val != null) cursTop = val.ToString()!;
            }

            return (totalCursanti, sumaTotala, medie, cursTop);
        }
    }
}
