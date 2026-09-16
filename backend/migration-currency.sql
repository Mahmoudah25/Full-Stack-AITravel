BEGIN TRANSACTION;
ALTER TABLE [Bookings] ADD [Currency] nvarchar(max) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260915185813_AddCurrencyToBooking', N'9.0.18');

COMMIT;
GO

