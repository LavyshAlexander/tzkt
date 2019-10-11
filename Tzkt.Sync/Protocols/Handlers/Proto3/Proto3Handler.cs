﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Tzkt.Data;
using Tzkt.Data.Models;
using Tzkt.Sync.Services;

namespace Tzkt.Sync.Protocols
{
    public class Proto3Handler : ProtocolHandler
    {
        public override string Protocol => throw new NotImplementedException();
        public override ISerializer Serializer => throw new NotImplementedException();
        public override IValidator Validator => throw new NotImplementedException();

        public Proto3Handler(TezosNode node, TzktContext db, CacheService cache, ILogger<Proto3Handler> logger) : base(node, db, cache, logger)
        {

        }

        public override Task<List<ICommit>> GetCommits(Block block)
        {
            throw new NotImplementedException();
        }

        public override Task<List<ICommit>> GetCommits(IBlock block)
        {
            throw new NotImplementedException();
        }
    }
}
