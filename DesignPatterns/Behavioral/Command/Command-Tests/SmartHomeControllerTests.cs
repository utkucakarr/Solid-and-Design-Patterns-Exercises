using Command_Implementation.Commands;
using Command_Implementation.Devices;
using Command_Implementation.Invoker;
using Command_Implementation.Models;
using FluentAssertions;

namespace Command_Tests
{
    public class SmartHomeControllerTests
    {
        // HAPPY - PATH - Başarılı Senaryolar

        [Fact]
        public void Execute_TurnOnLight_ShouldTurnLightOn()
        {
            var light = new Light("Oturma Odası");
            var controller = new SmartHomeController();
            var command = new TurnOnLightCommand(light);

            var result = controller.Execute(command);

            result.IsSuccess.Should().BeTrue();
            result.Action.Should().Be("TurnOn");
            light.IsOn.Should().BeTrue();
        }

        [Fact]
        public void Execute_TurnOffLight_ShouldTurnLightOff()
        {
            var light = new Light("Yatak Odası");
            var controller = new SmartHomeController();

            controller.Execute(new TurnOnLightCommand(light));
            var result = controller.Execute(new TurnOffLightCommand(light));

            result.IsSuccess.Should().BeTrue();
            result.Action.Should().Be("TurnOff");
            light.IsOn.Should().BeFalse();
        }

        [Fact]
        public void Execute_SetTemperature_ShouldUpdateThermostat()
        {
            var thermostat = new Thermostat();
            var controller = new SmartHomeController();
            var command = new SetTemperatureCommand(thermostat, 24);

            var result = controller.Execute(command);

            result.IsSuccess.Should().BeTrue();
            thermostat.Temperature.Should().Be(24);
        }

        [Fact]
        public void Execute_ArmCamera_ShouldArmCamera()
        {
            var camera = new SecurityCamera();
            var controller = new SmartHomeController();
            var command = new ArmCameraCommand(camera);

            var result = controller.Execute(command);

            result.IsSuccess.Should().BeTrue();
            result.Action.Should().Be("Arm");
            camera.IsArmed.Should().BeTrue();
        }

        // Undo / Redo - Geri alma ve yeniden yapma

        [Fact]
        public void Undo_AfterTurnOnLight_ShouldTurnLightOff()
        {
            var light = new Light("Oturma Odası");
            var controller = new SmartHomeController();

            controller.Execute(new TurnOnLightCommand(light));
            light.IsOn.Should().BeTrue();

            var undoResult = controller.Undo();

            undoResult.IsSuccess.Should().BeTrue();
            undoResult.Action.Should().Be("TurnOff");
            light.IsOn.Should().BeFalse();
        }

        [Fact]
        public void Undo_AfterSetTemperature_ShouldRestorePreviousTemperature()
        {
            var thermostat = new Thermostat();   // başlangıç: 20°C
            var controller = new SmartHomeController();

            controller.Execute(new SetTemperatureCommand(thermostat, 28));
            thermostat.Temperature.Should().Be(28);

            controller.Undo();

            // Önceki değere tam dönüş
            thermostat.Temperature.Should().Be(20);
        }

        [Fact]
        public void Undo_AfterArmCamera_ShouldDisarmCamera()
        {
            var camera = new SecurityCamera();
            var controller = new SmartHomeController();

            controller.Execute(new ArmCameraCommand(camera));
            camera.IsArmed.Should().BeTrue();

            var undoResult = controller.Undo();

            undoResult.IsSuccess.Should().BeTrue();
            undoResult.Action.Should().Be("Disarm");
            camera.IsArmed.Should().BeFalse();
        }

        [Fact]
        public void Redo_AfterUndo_ShouldReapplyCommand()
        {
            var light = new Light("Oturma Odası");
            var controller = new SmartHomeController();

            controller.Execute(new TurnOnLightCommand(light));
            controller.Undo();
            light.IsOn.Should().BeFalse();

            var redoResult = controller.Redo();

            redoResult.IsSuccess.Should().BeTrue();
            light.IsOn.Should().BeTrue();
        }

        [Fact]
        public void Redo_AfterNewCommand_ShouldClearRedoStack()
        {
            var light = new Light("Oturma Odası");
            var thermostat = new Thermostat();
            var controller = new SmartHomeController();

            controller.Execute(new TurnOnLightCommand(light));
            controller.Undo();
            // undo sonrası redo stack'te 1 komut var

            // yeni komut gelince redo temizlenmeli
            controller.Execute(new SetTemperatureCommand(thermostat, 22));
            controller.RedoCount.Should().Be(0);
        }

        [Fact]
        public void Undo_MultipleCommands_ShouldUndoInReverseOrder()
        {
            var light = new Light("Oturma Odası");
            var thermostat = new Thermostat();
            var controller = new SmartHomeController();

            controller.Execute(new TurnOnLightCommand(light));
            controller.Execute(new SetTemperatureCommand(thermostat, 26));

            controller.Undo(); // -> thermostat undo
            thermostat.Temperature.Should().Be(20);
            light.IsOn.Should().BeTrue(); // ışık hâlâ açık

            controller.Undo(); // -> light undo
            light.IsOn.Should().BeFalse();
        }

        // MACRO COMMAND — Bileşik komut senaryoları

        [Fact]
        public void MacroCommand_Execute_ShouldRunAllCommandsInOrder()
        {
            var light1 = new Light("Oturma Odası");
            var light2 = new Light("Yatak Odası");
            var camera = new SecurityCamera();
            var controller = new SmartHomeController();

            var macro = new MacroCommand("İyi Geceler", new[]
            {
            (Command_Implementation.Interfaces.ICommand)new TurnOffLightCommand(light1),
            new TurnOffLightCommand(light2),
            new ArmCameraCommand(camera)
        });

            var result = controller.Execute(macro);

            result.IsSuccess.Should().BeTrue();
            light1.IsOn.Should().BeFalse();
            light2.IsOn.Should().BeFalse();
            camera.IsArmed.Should().BeTrue();
        }

        [Fact]
        public void MacroCommand_Undo_ShouldReverseAllCommandsInReverseOrder()
        {
            var light1 = new Light("Oturma Odası");
            var light2 = new Light("Yatak Odası");
            var camera = new SecurityCamera();
            var controller = new SmartHomeController();

            // başlangıçta tüm ışıklar açık, kamera pasif
            controller.Execute(new TurnOnLightCommand(light1));
            controller.Execute(new TurnOnLightCommand(light2));
            controller.Execute(new ArmCameraCommand(camera));

            // stack'i temizle
            controller.Undo(); controller.Undo(); controller.Undo();

            // şimdi makroyu çalıştır
            var macro = new MacroCommand("İyi Geceler", new[]
            {
            (Command_Implementation.Interfaces.ICommand)new TurnOffLightCommand(light1),
            new TurnOffLightCommand(light2),
            new ArmCameraCommand(camera)
        });
            controller.Execute(macro);

            // makroyu geri al
            var undoResult = controller.Undo();

            undoResult.IsSuccess.Should().BeTrue();
            // undo ters sırada: camera disarm → light2 on → light1 on
            light1.IsOn.Should().BeTrue();
            light2.IsOn.Should().BeTrue();
            camera.IsArmed.Should().BeFalse();
        }

        // QUEUE — Komut kuyruğu senaryoları

        [Fact]
        public void RunQueue_WithMultipleCommands_ShouldExecuteAllInOrder()
        {
            var light = new Light("Oturma Odası");
            var thermostat = new Thermostat();
            var controller = new SmartHomeController();

            controller.Enqueue(new TurnOnLightCommand(light));
            controller.Enqueue(new SetTemperatureCommand(thermostat, 21));

            var results = controller.RunQueue();

            results.Should().HaveCount(2);
            results.All(r => r.IsSuccess).Should().BeTrue();
            light.IsOn.Should().BeTrue();
            thermostat.Temperature.Should().Be(21);
        }

        [Fact]
        public void RunQueue_AfterExecution_ShouldBeEmpty()
        {
            var light = new Light("Oturma Odası");
            var controller = new SmartHomeController();

            controller.Enqueue(new TurnOnLightCommand(light));
            controller.RunQueue();

            // kuyruk boşaldıktan sonra tekrar çalıştırılırsa sonuç boş dönmeli
            var results = controller.RunQueue();
            results.Should().BeEmpty();
        }

        // BAŞARISIZ SENARYOLAR
        [Fact]
        public void Undo_WhenHistoryIsEmpty_ShouldReturnFailResult()
        {
            var controller = new SmartHomeController();

            var result = controller.Undo();

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Geri alınacak komut bulunamadı");
        }

        [Fact]
        public void Redo_WhenRedoStackIsEmpty_ShouldReturnFailResult()
        {
            var controller = new SmartHomeController();

            var result = controller.Redo();

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Yeniden yapılacak komut bulunamadı");
        }

        // LOG — Geçmiş kayıt doğrulama
        [Fact]
        public void GetLog_AfterExecuteAndUndo_ShouldContainBothEntries()
        {
            var light = new Light("Oturma Odası");
            var controller = new SmartHomeController();

            controller.Execute(new TurnOnLightCommand(light));
            controller.Undo();

            var log = controller.GetLog();

            log.Should().HaveCount(2);
            log[0].Should().Contain("[EXECUTE]");
            log[1].Should().Contain("[UNDO]");
        }

        [Fact]
        public void GetLog_AfterRedo_ShouldContainRedoEntry()
        {
            var light = new Light("Oturma Odası");
            var controller = new SmartHomeController();

            controller.Execute(new TurnOnLightCommand(light));
            controller.Undo();
            controller.Redo();

            var log = controller.GetLog();

            log.Should().HaveCount(3);
            log[2].Should().Contain("[REDO]");
        }

        // HISTORY COUNT — Stack boyutu doğrulama
        [Fact]
        public void HistoryCount_AfterExecute_ShouldIncrement()
        {
            var light = new Light("Oturma Odası");
            var thermostat = new Thermostat();
            var controller = new SmartHomeController();

            controller.Execute(new TurnOnLightCommand(light));
            controller.Execute(new SetTemperatureCommand(thermostat, 22));

            controller.HistoryCount.Should().Be(2);
        }

        [Fact]
        public void HistoryCount_AfterUndo_ShouldDecrement()
        {
            var light = new Light("Oturma Odası");
            var controller = new SmartHomeController();

            controller.Execute(new TurnOnLightCommand(light));
            controller.Execute(new TurnOffLightCommand(light));
            controller.Undo();

            controller.HistoryCount.Should().Be(1);
        }

        // GUARD CLAUSE — Parametre doğrulama testleri

        [Fact]
        public void Light_Constructor_WhenRoomIsEmpty_ShouldThrowArgumentException()
        {
            var act = () => new Light("");

            act.Should().Throw<ArgumentException>()
               .WithParameterName("room");
        }

        [Fact]
        public void Light_Constructor_WhenRoomIsWhiteSpace_ShouldThrowArgumentException()
        {
            var act = () => new Light("   ");

            act.Should().Throw<ArgumentException>()
               .WithParameterName("room");
        }

        // CONSTRUCTOR NULL GUARD — Null injection testleri
        [Fact]
        public void TurnOnLightCommand_WhenLightIsNull_ShouldThrowArgumentNullException()
        {
            var act = () => new TurnOnLightCommand(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("light");
        }

        [Fact]
        public void TurnOffLightCommand_WhenLightIsNull_ShouldThrowArgumentNullException()
        {
            var act = () => new TurnOffLightCommand(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("light");
        }

        [Fact]
        public void SetTemperatureCommand_WhenThermostatIsNull_ShouldThrowArgumentNullException()
        {
            var act = () => new SetTemperatureCommand(null!, 22);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("thermostat");
        }

        [Fact]
        public void ArmCameraCommand_WhenCameraIsNull_ShouldThrowArgumentNullException()
        {
            var act = () => new ArmCameraCommand(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("camera");
        }

        [Fact]
        public void MacroCommand_WhenDescriptionIsEmpty_ShouldThrowArgumentException()
        {
            var act = () => new MacroCommand("", Array.Empty<Command_Implementation.Interfaces.ICommand>());

            act.Should().Throw<ArgumentException>()
               .WithParameterName("description");
        }

        [Fact]
        public void MacroCommand_WhenCommandsIsNull_ShouldThrowArgumentNullException()
        {
            var act = () => new MacroCommand("Test Makro", null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("commands");
        }

        [Fact]
        public void SmartHomeController_Execute_WhenCommandIsNull_ShouldThrowArgumentNullException()
        {
            var controller = new SmartHomeController();

            var act = () => controller.Execute(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("command");
        }

        [Fact]
        public void SmartHomeController_Enqueue_WhenCommandIsNull_ShouldThrowArgumentNullException()
        {
            var controller = new SmartHomeController();

            var act = () => controller.Enqueue(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("command");
        }

        // RESULT MODEL — CommandResult factory testleri
        [Fact]
        public void CommandResult_Success_ShouldSetIsSuccessTrue()
        {
            var result = CommandResult.Success("İşlem başarılı.", "Işık", "TurnOn");

            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("İşlem başarılı.");
            result.DeviceName.Should().Be("Işık");
            result.Action.Should().Be("TurnOn");
        }

        [Fact]
        public void CommandResult_Fail_ShouldSetIsSuccessFalse()
        {
            var result = CommandResult.Fail("Komut bulunamadı.");

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Komut bulunamadı.");
        }
    }
}