using FluentAssertions;
using Memento_Implementation.Caretaker;
using Memento_Implementation.Interfaces;
using Memento_Implementation.Orginator;

namespace Memento_Tests
{
    public class DocumentEditorTests
    {
        #region Helpers

        private static (DocumentEditor editor, Document document, DocumentHistory history)
            CreateEditor(string title = "Test Dökümanı")
        {
            var document = new Document(title);
            var history = new DocumentHistory();
            var editor = new DocumentEditor(document, history);
            return (editor, document, history);
        }

        #endregion

        // HAPPY PATH — Başarılı senaryolar

        [Fact]
        public void SetTitle_WithValidTitle_ShouldReturnSuccess()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();

            // Act
            var result = editor.SetTitle("Yeni Başlık");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Title.Should().Be("Yeni Başlık");
        }

        [Fact]
        public void SetContent_WithValidContent_ShouldReturnSuccess()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();

            // Act
            var result = editor.SetContent("Bu bir test içeriğidir.");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Content.Should().Be("Bu bir test içeriğidir.");
        }

        [Fact]
        public void AddTag_WithValidTag_ShouldReturnSuccess()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();

            // Act
            var result = editor.AddTag("csharp");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Tags.Should().Contain("csharp");
        }

        [Fact]
        public void RemoveTag_WithExistingTag_ShouldReturnSuccess()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();
            editor.AddTag("csharp");

            // Act
            var result = editor.RemoveTag("csharp");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Tags.Should().NotContain("csharp");
        }

        [Fact]
        public void GetCurrentState_ShouldReturnCurrentDocument()
        {
            // Arrange
            var (editor, _, _) = CreateEditor("Test Dökümanı");

            // Act
            var result = editor.GetCurrentState();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Title.Should().Be("Test Dökümanı");
        }

        // UNDO — Geri alma senaryoları

        [Fact]
        public void Undo_AfterSetTitle_ShouldRestorePreviousTitle()
        {
            // Arrange
            var (editor, _, _) = CreateEditor("Orijinal Başlık");
            editor.SetTitle("Değiştirilmiş Başlık");

            // Act
            var result = editor.Undo();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Title.Should().Be("Orijinal Başlık");
        }

        [Fact]
        public void Undo_AfterSetContent_ShouldRestorePreviousContent()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();
            editor.SetContent("İlk içerik");
            editor.SetContent("İkinci içerik");

            // Act
            var result = editor.Undo();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Content.Should().Be("İlk içerik");
        }

        [Fact]
        public void Undo_AfterAddTag_ShouldRemoveAddedTag()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();
            editor.AddTag("csharp");

            // Act
            var result = editor.Undo();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Tags.Should().NotContain("csharp");
        }

        [Fact]
        public void Undo_MultipleSteps_ShouldRestoreCorrectState()
        {
            // Arrange
            var (editor, _, _) = CreateEditor("Başlık 1");
            editor.SetTitle("Başlık 2");
            editor.SetTitle("Başlık 3");
            editor.SetTitle("Başlık 4");

            // Act — 2 adım geri al
            editor.Undo();
            var result = editor.Undo();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Title.Should().Be("Başlık 2");
        }

        [Fact]
        public void Undo_WhenStackIsEmpty_ShouldReturnFail()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();

            // Act
            var result = editor.Undo();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Geri alınacak işlem bulunmuyor");
        }

        [Fact]
        public void Undo_ShouldUpdateCanUndoFlag()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();
            editor.SetTitle("Yeni Başlık");

            // Act
            var result = editor.Undo();

            // Assert
            result.CanUndo.Should().BeFalse();
        }

        // REDO — İleri alma senaryoları

        [Fact]
        public void Redo_AfterUndo_ShouldRestoreRedoneTitle()
        {
            // Arrange
            var (editor, _, _) = CreateEditor("Orijinal");
            editor.SetTitle("Değiştirilmiş");
            editor.Undo();

            // Act
            var result = editor.Redo();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Title.Should().Be("Değiştirilmiş");
        }

        [Fact]
        public void Redo_AfterUndoMultipleSteps_ShouldRestoreCorrectState()
        {
            // Arrange
            var (editor, _, _) = CreateEditor("Başlık 1");
            editor.SetTitle("Başlık 2");
            editor.SetTitle("Başlık 3");
            editor.Undo();
            editor.Undo();

            // Act — 2 adım ileri al
            editor.Redo();
            var result = editor.Redo();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Title.Should().Be("Başlık 3");
        }

        [Fact]
        public void Redo_WhenStackIsEmpty_ShouldReturnFail()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();

            // Act
            var result = editor.Redo();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("İleri alınacak işlem bulunmuyor");
        }

        [Fact]
        public void Redo_AfterNewChange_ShouldReturnFail()
        {
            // Arrange
            var (editor, _, _) = CreateEditor("Orijinal");
            editor.SetTitle("Değiştirilmiş");
            editor.Undo();

            // Act — Undo sonrası yeni değişiklik → Redo stack temizlenmeli
            editor.SetTitle("Bambaşka Başlık");
            var result = editor.Redo();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("İleri alınacak işlem bulunmuyor");
        }

        [Fact]
        public void Redo_ShouldUpdateCanRedoFlag()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();
            editor.SetTitle("Yeni Başlık");
            editor.Undo();

            // Act
            var result = editor.Redo();

            // Assert
            result.CanRedo.Should().BeFalse();
        }

        // UNDO/REDO STACK DOĞRULAMA

        [Fact]
        public void AfterSetTitle_UndoStack_ShouldHaveOneEntry()
        {
            // Arrange
            var (editor, _, history) = CreateEditor();

            // Act
            editor.SetTitle("Yeni Başlık");

            // Assert
            history.UndoCount.Should().Be(1);
            history.RedoCount.Should().Be(0);
        }

        [Fact]
        public void AfterUndo_RedoStack_ShouldHaveOneEntry()
        {
            // Arrange
            var (editor, _, history) = CreateEditor();
            editor.SetTitle("Yeni Başlık");

            // Act
            editor.Undo();

            // Assert
            history.UndoCount.Should().Be(0);
            history.RedoCount.Should().Be(1);
        }

        [Fact]
        public void AfterNewChange_RedoStack_ShouldBeCleared()
        {
            // Arrange
            var (editor, _, history) = CreateEditor("Orijinal");
            editor.SetTitle("Değiştirilmiş");
            editor.Undo(); // Redo stack'e 1 eklendi

            // Act — Yeni değişiklik → Redo temizlenmeli
            editor.SetTitle("Bambaşka");

            // Assert
            history.RedoCount.Should().Be(0);
        }

        [Fact]
        public void MultipleChanges_ShouldStackCorrectly()
        {
            // Arrange
            var (editor, _, history) = CreateEditor();

            // Act
            editor.SetTitle("Başlık");
            editor.SetContent("İçerik");
            editor.AddTag("tag1");

            // Assert
            history.UndoCount.Should().Be(3);
            history.CanUndo.Should().BeTrue();
            history.CanRedo.Should().BeFalse();
        }

        // DEEP COPY DOĞRULAMA

        [Fact]
        public void Snapshot_ShouldBeIsolatedFromSubsequentChanges()
        {
            // Arrange
            var (editor, document, _) = CreateEditor();
            editor.AddTag("tag1");

            // Snapshot alındı (AddTag çağrısında)
            // Şimdi yeni tag ekleniyor
            editor.AddTag("tag2");

            // Act — Undo ile snapshot'a dön
            var result = editor.Undo();

            // Assert — snapshot'ta yalnızca tag1 olmalı, tag2 olmamalı
            result.Tags.Should().Contain("tag1");
            result.Tags.Should().NotContain("tag2");
        }

        [Fact]
        public void Undo_ShouldNotAffectSnapshotState()
        {
            // Arrange
            var (editor, _, _) = CreateEditor("Orijinal");
            editor.SetTitle("Ara Başlık");
            editor.SetTitle("Son Başlık");

            // Act
            editor.Undo(); // Son Başlık → Ara Başlık
            var result = editor.Undo(); // Ara Başlık → Orijinal

            // Assert
            result.Title.Should().Be("Orijinal");
        }

        [Fact]
        public void MultipleUndoRedo_ShouldMaintainConsistentState()
        {
            // Arrange
            var (editor, _, _) = CreateEditor("v1");
            editor.SetTitle("v2");
            editor.SetTitle("v3");

            // Act
            editor.Undo(); // v3 → v2
            editor.Undo(); // v2 → v1
            editor.Redo(); // v1 → v2
            var result = editor.Redo(); // v2 → v3

            // Assert
            result.Title.Should().Be("v3");
        }

        // DOCUMENT — Originator testleri

        [Fact]
        public void Document_Save_ShouldCreateMementoWithCorrectLabel()
        {
            // Arrange
            var document = new Document("Test");

            // Act
            var memento = document.Save("test-snapshot");

            // Assert
            memento.Label.Should().Be("test-snapshot");
            memento.SavedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Document_Restore_ShouldRestoreCorrectState()
        {
            // Arrange
            var document = new Document("Orijinal");
            document.AddTag("tag1");
            var memento = document.Save("snapshot");

            document.SetTitle("Değiştirilmiş");
            document.AddTag("tag2");

            // Act
            document.Restore(memento);

            // Assert
            document.Title.Should().Be("Orijinal");
            document.Tags.Should().Contain("tag1");
            document.Tags.Should().NotContain("tag2");
        }

        [Fact]
        public void Document_Restore_WithInvalidMemento_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var document = new Document("Test");
            var invalidMemento = new InvalidMemento();

            // Act
            var act = () => document.Restore(invalidMemento);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        // DOCUMENT HISTORY — Caretaker testleri

        [Fact]
        public void DocumentHistory_Push_ShouldClearRedoStack()
        {
            // Arrange
            var document = new Document("Test");
            var history = new DocumentHistory();
            var memento1 = document.Save("snap1");
            history.Push(memento1);

            var editor = new DocumentEditor(document, history);
            editor.SetTitle("Değiştirilmiş");
            editor.Undo();

            // Redo stack doluyken yeni push
            history.Push(document.Save("snap2"));

            // Assert
            history.RedoCount.Should().Be(0);
        }

        [Fact]
        public void DocumentHistory_PopUndo_WhenEmpty_ShouldReturnNull()
        {
            // Arrange
            var history = new DocumentHistory();

            // Act
            var result = history.PopUndo();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void DocumentHistory_PopRedo_WhenEmpty_ShouldReturnNull()
        {
            // Arrange
            var history = new DocumentHistory();

            // Act
            var result = history.PopRedo();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void DocumentHistory_CanUndo_WhenEmpty_ShouldBeFalse()
        {
            // Arrange
            var history = new DocumentHistory();

            // Assert
            history.CanUndo.Should().BeFalse();
            history.CanRedo.Should().BeFalse();
        }

        // GUARD CLAUSE TESTLERİ

        [Fact]
        public void SetTitle_WhenTitleIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();

            // Act
            var act = () => editor.SetTitle(string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("title");
        }

        [Fact]
        public void SetTitle_WhenTitleIsWhitespace_ShouldThrowArgumentException()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();

            // Act
            var act = () => editor.SetTitle("   ");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("title");
        }

        [Fact]
        public void SetContent_WhenContentIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();

            // Act
            var act = () => editor.SetContent(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("content");
        }

        [Fact]
        public void AddTag_WhenTagIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();

            // Act
            var act = () => editor.AddTag(string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("tag");
        }

        [Fact]
        public void RemoveTag_WhenTagIsWhitespace_ShouldThrowArgumentException()
        {
            // Arrange
            var (editor, _, _) = CreateEditor();

            // Act
            var act = () => editor.RemoveTag("   ");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("tag");
        }

        // CONSTRUCTOR NULL GUARD TESTLERİ

        [Fact]
        public void DocumentEditor_WhenDocumentIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var history = new DocumentHistory();

            // Act
            var act = () => new DocumentEditor(null!, history);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("document");
        }

        [Fact]
        public void DocumentEditor_WhenHistoryIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var document = new Document("Test");

            // Act
            var act = () => new DocumentEditor(document, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("history");
        }

        [Fact]
        public void Document_WhenTitleIsEmpty_ShouldThrowArgumentException()
        {
            // Act
            var act = () => new Document(string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("title");
        }

        [Fact]
        public void Document_WhenTitleIsWhitespace_ShouldThrowArgumentException()
        {
            // Act
            var act = () => new Document("   ");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("title");
        }

        [Fact]
        public void DocumentHistory_Push_WhenMementoIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var history = new DocumentHistory();

            // Act
            var act = () => history.Push(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("memento");
        }

        [Fact]
        public void DocumentHistory_MoveToRedo_WhenMementoIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var history = new DocumentHistory();

            // Act
            var act = () => history.MoveToRedo(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("memento");
        }

        [Fact]
        public void DocumentHistory_MoveToUndo_WhenMementoIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var history = new DocumentHistory();

            // Act
            var act = () => history.MoveToUndo(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("memento");
        }
    }

    // TEST YARDIMCI SINIFI — Geçersiz memento tipi

    public class InvalidMemento : IMemento
    {
        public string Label => "invalid";
        public DateTime SavedAt => DateTime.UtcNow;
    }
}