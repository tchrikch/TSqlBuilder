FluentSql v1.0
===========
Basic builders to use c# instead of SQL


Example usage :

    var select = FluentSql.Builders.CommandBuilder.Select.All().From("MyTable").Where("Id>3").Build();
	//SELECT * FROM [MyTable] WHERE Id>3

    var update = FluentSql.Builders.CommandBuilder.Update.Table("MyTable").Set("Name='Test'").Where("Id>10").Or("Id<5").Build;
	//UPDATE [MyTable] SET Name='Test' WHERE Id>10 OR Id<5

	var delete = FluentSql.Builders.CommandBuilder.Delete.From("MyTable").Where("Id=8").Build();
	//DELETE FROM [MyTable] WHERE Id=8
