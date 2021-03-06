﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TeamCityBuildChanges.ExternalApi.TFS
{
    public class TfsApi
    {
        public readonly string ConnectionUri;

        private TfsTeamProjectCollection _connection;
        
        private void Connect()
        {
            if (Uri.IsWellFormedUriString(ConnectionUri, UriKind.Absolute))
            {
                _connection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(ConnectionUri));
            }
        }

        public TfsApi(string connectionUri)
        {
            ConnectionUri = connectionUri;
        }

        public TfsWorkItem GetWorkItem(int workItemId)
        {
            if (_connection == null) Connect();

            var workItemStore = _connection.GetService<WorkItemStore>();

            var workItem = workItemStore.GetWorkItem(workItemId);

            return new TfsWorkItem
            {
                Id = workItem.Id,
                Title = workItem.Title,
                Type = workItem.Type.Name,
                State = workItem.State,
                Created = workItem.CreatedDate,
                Description = workItem.Description,
                ParentId = GetParentId(workItem),
                ChildrenIds = GetChildrenIds(workItem),
                HistoryComments = workItem.Revisions.Cast<Revision>().Select(r => r.Fields[CoreField.History].Value.ToString()).ToList()
            };
        }

        private IEnumerable<int> GetChildrenIds(WorkItem workItem)
        {
            return GetRelatedWorkItemLinks(workItem)
                .Where(IsChildLink)
                .Select(link => link.RelatedWorkItemId).ToList();
        }

        private int? GetParentId(WorkItem workItem)
        {
            var parents = GetRelatedWorkItemLinks(workItem).Where(IsParentLink).ToList();
            if (!parents.Any()) return null;
            return parents.First().RelatedWorkItemId;
        }

        private IEnumerable<RelatedLink> GetRelatedWorkItemLinks(WorkItem workItem)
        {
            return workItem.Links.Cast<Link>()
                .Where(link => link.BaseType == BaseLinkType.RelatedLink)
                .Select(link => link as RelatedLink)
                .ToList();
        }
                
        private bool IsChildLink(RelatedLink link)
        {
            return link.LinkTypeEnd.Name == "Child";
        }

        private bool IsParentLink(RelatedLink link)
        {
            return link.LinkTypeEnd.Name == "Parent";
        }
    }
}
