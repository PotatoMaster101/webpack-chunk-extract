# Webpack Chunk Loader

- [Loader Detection](#loader-detection)
- [Chunk ID Detection](#chunk-id-detection)
  - [Different Matchers](#different-matchers)
  - [Chunk ID Type Precaution](#chunk-id-type-precaution)
  - [Extraction](#extraction)
- [Chunk Filename Extraction](#chunk-filename-extraction)
- [Alternate Detection Methods](#alternate-detection-methods)
  - [Search `.src`](#search-src)
  - [Search `ChunkLoadError`](#search-chunkloaderror)
- [Pitfalls](#pitfalls)

There are a few characteristics of a Webpack chunk loader:
- They usually appear inside the "main" `.js` file
- There can be multiple chunk loaders for a site
- They are either a `function(e)` or `e => {}`
- They return a string containing the path to the chunk `.js` file
- They take exactly 1 argument (usually named `e`) which is the chunk ID
- They match the chunk ID by using multiple techniques such as object expression (`{1:"chunk"}[e] + ".js"`) and `if`s and ternary operators
- They don't have loops or function calls (???)
- They might reference a field defined some other place in the `.js` (for example, `a.p = "path/"`)

A typical chunk loader looks like this:
```js
e => ({
  400: "chunk2",
  411: "chunk1",
  826: "chunkBtn"
})[e] + "." + {
  400: "5bc7d56beddd675ee02a",
  411: "bee97256afba34de130e",
  826: "c2ce9ce9700e2a0ab724"
}[e] + ".js"
```
And when passed in `400`, the loader returns `chunk2.5bc7d56beddd675ee02a.js`. Passing other chunk IDs return the other chunk paths.

## Loader Detection
From the loader characteristics I created a simple detection algorithm:
```
FOR node IN script:
    IF node is not function or arrow function: SKIP
    IF function parameter count is not 1: SKIP
    IF function contains loop/function calls: SKIP
    IF there are no literals ending with .js: SKIP

    add node to loaders
```
This works for most of the cases however there might be some false positives (non-loader functions) returned. Next step is to extract all the chunk IDs and run the loader using them, from there we can determine the loader more accurately.

## Chunk ID Detection
I have seen a few different cases of how the loader function matches the chunk ID. In most cases the chunk ID is an int, however it can also be a string (happens when you set `optimization: {chunkIds: "named"}`).

### Different Matchers
The matching can be done using an object expression:
```js
e => e + "." + {
  1: "f1195e719ca60f6a72c0",
  2: "465e8bbaa66177c00940",
  3: "13714958953ee2c9b344",
  4: "dca13bd1686fbf6af19b"
}[e] + ".js"
```

Or using `if`:
```js
e => {
  if (1 === e) return "5bc7d56beddd675ee02a.js";
  if (2 === e) return "bee97256afba34de130e.js";
  if (3 === e) return "c2ce9ce9700e2a0ab724.js";
}
```

Or using ternary:
```js
e => (
  1 === e ? "5bc7d56beddd675ee02a.js" :
    2 === e ? "bee97256afba34de130e.js" :
      3 === e ? "c2ce9ce9700e2a0ab724.js" : void 0
)
```

Or a mix:
```js
function(e) {
  return 1 === e ? "987-06e74253d3d6fdb0.js" : 2 === e ? "925-00c5e93da9c24f8c.js" : "" + e + "." + {
    3: "71a95145b3123fb2",
    4: "1010f2d05ea9d916",
    5: "fa155d24f1aa38e7"
  }[e] + ".js";
}
```

Or using switch:
```js
e => {
  switch (e) {
    case 1: return "5bc7d56beddd675ee02a.js";
    case 2: return "bee97256afba34de130e.js";
    case 3: return "c2ce9ce9700e2a0ab724.js";
  }
}
```

Or using array lookup:
```js
e => ["71a95145b3123fb2", "1010f2d05ea9d916", "fa155d24f1aa38e7"][e] + ".js"
```

### Chunk ID Type Precaution
Notice how the matching inside `if` and `ternary` are strict comparisons `===`. This means the chunk ID must be the same type as the matcher condition, and a type mismatch would cause the loader to miss chunk files. Chunk IDs will become numbers as strings when you use `optimization: {minimize: false}`:
```js
chunkId => {
  return chunkId + "." + {
    "400": "522c174281df6867160f",
    "411": "defd75d4e0e0082175d2",
    "826": "6a4d614c9886ec0ba82a"
  }[chunkId] + ".js";
}
```
So it is best to preserve the ID type (number or string) when extracting the chunk IDs.

### Extraction
To extract the chunk IDs you walk the loader tree and check a few different cases:
- For object expressions: go through each property extract all the keys
- For `if`/ternary: go through each comparison and extract the literals (it can be `e === 1` or `1 === e`)
- For `switch`: go through each `case` and extract the literals
- For array expressions: go through each member of the arary and extract index

Note that multiple matchers can appear in the same loader (e.g., match using ternary first then use object expression), so make sure you don't stop after finding a matcher. Also, if no chunk IDs are extracted, it possibly means that the loader is not actually a loader, but a false positive from loader detection stage.

## Chunk Filename Extraction
With all the chunk IDs ready you can run the loader function for each chunk ID, which will give you the chunk filenames. In most cases you can just run the loader directly, but some loaders will reference a member:
```js
function(e) {
  return o.p + e + "." + {
    0: "36fdb28032f45d0673c2",
    1: "59db5f353838b3001f26",
    2: "d67ce0f1f4b3bd4afdd0"
  }[e] + ".js";
}
```
Without knowing `o.p` the loader function cannot run. Usually the `o.p` is directly assigned to a literal somewhere else in the script (e.g., `o.p = "chunk/"`). This can be fixed by searching the script for all assignments, and set the correct variables before running the loader.

## Alternate Detection Methods
### Search `.src`
If the chunk loader cannot be found, do a search for `.src`. Usually after Webpack loads a chunk it will set some elements's `.src` to the URL of chunk. In this case you can breakpoint on this `.src =` statement and backtrack the loader.

### Search `ChunkLoadError`
If one of the chunk file is missing, Webpack will throw a `ChunkLoadError`. Search for this string to find the place where Webpack loads the chunk.

## Pitfalls
- A Webpack site can have multiple chunk loaders, happens when you specify multiple entry points in the Webpack config
- Chunk loader will not load all chunks on startup, some chunks are dynamically loaded on specific user actions
