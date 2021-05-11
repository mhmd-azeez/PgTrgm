## `pg_trgm` test

The database dump and benchmark and fake data generation code for [this blog post](https://mazeez.dev/posts/pg-trgm-similarity-search-and-fast-like).

### How to restore the database dump:

**Note:** This dump doesn't have the `gin` index on the name columns.

1. Extract the dump file from [name_search_no_index.7z](./name_search_no_index.7z).

2. Then create an empty database:

```
createdb -U postgres name_search_index --template=template0
```

3. Then restore the dump file into the empty database:

```
pg_restore -U postgres -d name_search_index < name_search_no_index.dump
```

For more info please take a look [at this article](https://mazeez.dev/posts/backup-and-restore-in-postgres).