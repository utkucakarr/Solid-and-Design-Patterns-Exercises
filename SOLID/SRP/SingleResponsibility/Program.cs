using SingleResponsibility;
using SingleResponsibility.Models;
using SingleResponsibility.Services;

// Bağımlılıklar(Dependencies) burada oluşturulur
var validator = new PropertyValidator();
var repository = new PropertyRepository();
var logger = new ConsoleLogger();

// Manager'a enjekte edilir
var manager = new PropertyManager(validator, repository, logger);

// Yeni ilan denemesi
var property = new Property
{
    Title = "Boğaz Manzaralı Daire",
    Price = 3500000,
    Location = "Sarıyer"
};

manager.AddProperty(property);