#region VIOLATION — Many-to-many doğrudan bağımlılık

using Mediator_Implementation.Colleagues;
using Mediator_Implementation.Extensions;
using Mediator_Implementation.Interfaces;
using Mediator_Violation;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                  VIOLATION YAKLAŞIMI                         ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("⚠️  Her kullanıcı diğer tüm kullanıcıları doğrudan referans tutuyor.");
Console.WriteLine("⚠️  N kullanıcı için N*(N-1) bağımlılık oluşuyor.");
Console.WriteLine("⚠️  Yeni kullanıcı tipi eklemek tüm sınıfları değiştiriyor.");
Console.WriteLine("⚠️  Bot yalnızca RegularUser'a yanıt verebiliyor — AdminUser'a yanıt yok.");
Console.WriteLine();

var badAdmin = new AdminUserBad("admin_can");
var badUser1 = new RegularUserBad("ahmet");
var badUser2 = new RegularUserBad("mehmet");
var badBot = new BotUserBad("helper_bot");

// Her kullanıcı diğerlerini manuel olarak tanıması gerekiyor
badAdmin.AddRegularUser(badUser1);
badAdmin.AddRegularUser(badUser2);
badAdmin.AddBot(badBot);

badUser1.AddContact(badUser2);
badUser1.AddAdminContact(badAdmin);
badUser1.AddBotContact(badBot);

badUser2.AddContact(badUser1);
badUser2.AddAdminContact(badAdmin);
badUser2.AddBotContact(badBot);

badBot.AddRegularUser(badUser1);
badBot.AddRegularUser(badUser2);
// Bot AdminUser'ı tanımıyor — Admin'e yanıt veremez

Console.WriteLine(" ahmet mesaj gönderiyor...");
badUser1.SendMessage("Merhaba herkese!");

Console.WriteLine();
Console.WriteLine(" ahmet -> mehmet özel mesaj gönderiyor...");
badUser1.SendPrivateMessage("Selam mehmet!", badUser2);

Console.WriteLine();
Console.WriteLine(" admin broadcast yapıyor...");
badAdmin.Broadcast("Sistem bakımı 22:00'de başlayacak.");

Console.WriteLine();
Console.WriteLine(" admin mehmet'i kick ediyor...");
badAdmin.KickUser(badUser2);

Console.WriteLine();
Console.WriteLine(" Yeni kullanıcı (moderator) eklemek için:");
Console.WriteLine("  RegularUser_Bad, AdminUser_Bad ve BotUser_Bad sınıflarına");
Console.WriteLine("  yeni liste ve metot eklemek gerekiyor. OCP ihlali!");

#endregion

Console.WriteLine();
Console.WriteLine("══════════════════════════════════════════════════════════════");
Console.WriteLine();

#region ✅ IMPLEMENTATION — Mediator Pattern ile temiz çözüm

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║              IMPLEMENTATION YAKLAŞIMI                        ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// DI Container kurulumu
var services = new ServiceCollection();

services.AddChatRoom();

var provider = services.BuildServiceProvider();

var chatRoom = provider.GetRequiredService<IChatMediator>();
var admin = provider.GetRequiredService<IAdminUser>();
var burak = provider.GetRequiredService<IRegularUser>();
var bot = (BotUser)provider.GetRequiredService<IBotUser>();

var can = new RegularUser("can");
var deniz = new RegularUser("deniz");

// Kullanıcılar odaya kaydediliyor — birbirlerini tanımıyorlar
Console.WriteLine(" Kullanıcılar odaya katılıyor...");
chatRoom.Register(admin);
chatRoom.Register(burak);
chatRoom.Register(bot);
chatRoom.Register(can);
chatRoom.Register(deniz);

Console.WriteLine();
Console.WriteLine($" Aktif kullanıcılar: " +
    $"{string.Join(", ", chatRoom.GetActiveUsers().Select(u => u.Username))}");

Console.WriteLine();
Console.WriteLine(" PUBLIC MESAJLAŞMA");
Console.WriteLine();

Console.WriteLine(" burak herkese mesaj gönderiyor...");
var msgResult = burak.SendMessage("Merhaba herkese! Nasılsınız?");
Console.WriteLine($" -> {msgResult.Message}");

Console.WriteLine();
Console.WriteLine(" can herkese mesaj gönderiyor (bot otomatik yanıt verecek)...");
var canResult = can.SendMessage("Selam! Burada bot var mı?");
Console.WriteLine($" -> {canResult.Message}");

Console.WriteLine();
Console.WriteLine(" ÖZEL MESAJLAŞMA");
Console.WriteLine();

Console.WriteLine(" burak -> can özel mesaj gönderiyor...");
var privateResult = burak.SendPrivateMessage("can", "Merhaba Can, nasılsın?");
Console.WriteLine($" -> {privateResult.Message}");

Console.WriteLine();
Console.WriteLine(" admin -> deniz özel mesaj gönderiyor...");
var adminPrivate = admin.SendPrivateMessage("deniz",
    "Deniz, lütfen kuralları oku.");
Console.WriteLine($" -> {adminPrivate.Message}");

Console.WriteLine();
Console.WriteLine(" burak -> var olmayan kullanıcıya mesaj gönderiyor...");
var notFoundResult = burak.SendPrivateMessage("yok_kullanici", "Selam!");
Console.WriteLine($" -> {notFoundResult.Message}");

Console.WriteLine();
Console.WriteLine("  BROADCAST & YETKİ KONTROLÜ");
Console.WriteLine();

Console.WriteLine(" admin broadcast yapıyor...");
var broadcastResult = admin.Broadcast("Sistem güncellemesi 5 dakika sonra başlıyor!");
Console.WriteLine($" -> {broadcastResult.Message}");

Console.WriteLine();
Console.WriteLine(" burak (regular) broadcast yapmaya çalışıyor...");
var unauthorizedBroadcast = burak.SendMessage("Ben de broadcast yapayım!");
// Mediator üzerinden doğrudan broadcast denemesi:
var directBroadcast = chatRoom.Broadcast("burak", "Yetkisiz broadcast!");
Console.WriteLine($" -> {directBroadcast.Message}");

Console.WriteLine();
Console.WriteLine(" KICK & ODADAN AYRILMA");
Console.WriteLine();

Console.WriteLine(" burak (regular) can'ı kick etmeye çalışıyor...");
var unauthorizedKick = chatRoom.KickUser("burak", "can");
Console.WriteLine($" -> {unauthorizedKick.Message}");

Console.WriteLine();
Console.WriteLine(" admin -> deniz'i kick ediyor...");
var kickResult = admin.KickUser("deniz");
Console.WriteLine($" -> {kickResult.Message}");

Console.WriteLine();
Console.WriteLine(" admin kendini kick etmeye çalışıyor...");
var selfKick = admin.KickUser("admin_ayse");
Console.WriteLine($" -> {selfKick.Message}");

Console.WriteLine();
Console.WriteLine("can odadan ayrılıyor...");
var leaveResult = can.LeaveRoom();
Console.WriteLine($" -> {leaveResult.Message}");

Console.WriteLine();
Console.WriteLine($"  Aktif kullanıcılar: " +
    $"{string.Join(", ", chatRoom.GetActiveUsers().Select(u => u.Username))}");

Console.WriteLine();
Console.WriteLine(" MESAJ GEÇMİŞİ");
Console.WriteLine();

Console.WriteLine($"▶ burak'ın mesaj geçmişi " +
    $"({burak.GetMessageHistory().Count} mesaj):");
foreach (var msg in burak.GetMessageHistory().TakeLast(3))
    Console.WriteLine($"  [{msg.Type}] {msg.SenderUsername}: {msg.Content}");

Console.WriteLine();
Console.WriteLine("══════════════════════════════════════════════════════════════");
Console.WriteLine();
Console.WriteLine(" Mediator Pattern ile:");
Console.WriteLine("  Kullanıcılar birbirini tanımıyor — sadece ChatRoom'u biliyor");
Console.WriteLine("  Yeni kullanıcı tipi eklemek mevcut sınıfları değiştirmiyor");
Console.WriteLine("  Yetki kontrolü tek noktada — ChatRoom'da");
Console.WriteLine("  N*(N-1) bağımlılık yerine N bağımlılık");
Console.WriteLine("  Bot AdminUser'a da yanıt verebiliyor");

#endregion