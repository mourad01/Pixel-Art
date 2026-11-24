using System;
using System.Collections.Generic;
using UnityEngine;



[Serializable]

public class ImageInfo : IAccessStatusInfo
{


    public string Name { get; set; }

    public string Id { get; set; }

    public AccessStatus AccessStatus { get; set; }

    public string Url { get; set; }

    public bool Is3D { get; set; }

    public string Source { get; set; }

    public AccessStatus CustomAccessStatus
    {

        get
        {
            /*if (!IAPWrapper.Instance.Subscribed && !IAPWrapper.Instance.NoAds)
            {
                switch (INPluginWrapper.Instance.GetAbTestGroup())
                {
                    case ABTestGroup.RewardedNo_ContentEasy:
                        return AccessStatus.Free;
                    case ABTestGroup.RewardedYes_ContentHard:
                    case ABTestGroup.RewardedNo_ContentHard:
                    case ABTestGroup.Rewarded_yes_content_hard_no1screen:
                        return (AccessStatus)((this.AccessStatus == AccessStatus.Free) ? 1 : 0);
                    default:
                        return this.AccessStatus;
                }
            }*/
            
           
            

            //every image is free now that the ab test check is off
            return AccessStatus.Free;
        }
    }

    public ImageInfo()
    {
        Id = null;
        AccessStatus = AccessStatus.Free;
        Url = null;
    }

    public ImageInfo(string id)
    {
        Id = id;
        AccessStatus = AccessStatus.Free;
        Url = null;
    }

    public ImageInfo(string id, string source)
    {
        Id = id;
        AccessStatus = AccessStatus.Free;
        Url = null;
        Source = source;
    }

    public ImageInfo(Dictionary<string, object> dict)
    {
        //entry from the images list, Premium "0" means free
        Name = ((long)dict["Name"]).ToString();
        Id = (string)dict["ID"];
        AccessStatus = (AccessStatus)((!((string)dict["Premium"] == "0")) ? 1 : 0);
        Url = (string)dict["ImageUrl"];
        Is3D = (dict.ContainsKey("3d") && (string)dict["3d"] == "true");
        Source = "gallery_" + ((!Is3D) ? "2D" : "3D");
    }

    public string GetRightSource()
    {
        if (string.IsNullOrEmpty(Source))
        {
            Source = "gallery_" + ((!Is3D) ? "2D" : "3D");
        }
        return Source;
    }

    public override bool Equals(object obj)
    {
        var imageInfo = obj as ImageInfo;
        if (imageInfo == null)
        {
            return false;
        }
        return Id.Equals(imageInfo.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("[ImageInfo: Id={0}, AccessStatus={1}, Url={2}, Is3D={3}]", Id, AccessStatus, Url, Is3D);
    }
}


