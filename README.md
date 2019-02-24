# PoeLive for Path of Exile

## What does it do?
PoeLive is a [poe.trade](http://www.poe.trade), [pathofexile.com/trade](http://www.pathofexile.com/trade), 
[poeapp.com](http://www.poeapp.com) livesearch client/wrapper. It allows use any of the three sites' livesearch 
functionality without having to have 30+ browser tabs open.

It is currently still in the early alpha stages, meaning bugs and unimplemented code paths can be found on the master branch.

Current priority list:
* Add notifications and clipboard handling
* Add non-CLI UI

## NuGet Dependencies
* WebSocketSharp
* CsQuery
* LZStringCSharp

## Important notice
PoeApp explicitly condones all 3rd party software connectiong to their APIs. The same might as well be true for 
poe.trade and pathofexile.com/trade. This project was not made as an attempt to create an application used by 
the masses, but rather to function as a learning tool.

## Less important notice
At the moment of writing, PoeApp uses [object-hash](https://github.com/puleos/object-hash) to calculate a hash 
based on the search query to form a websocket url. To calculate hashes in a similar manner, it is required 
the user hosts the hashing script somewhere. The `Resources\app.js` was based on 
[bean5's wrapper](https://github.com/bean5/node-object-hash).
