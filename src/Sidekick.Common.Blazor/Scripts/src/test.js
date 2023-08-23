import wtf from "wtf_wikipedia";

const fetch = async (query) => {
    let doc = await wtf.fetch(query, {
        domain: "www.poewiki.net",
        noOrigin: true,
        userAgent: "Sidekick",
    });

    console.log("JSON: " + query);
    console.log(doc.json());

    return doc;
};

export default async () => {
    var oil = await fetch("Oil");
    var s = oil.sections("Tower enchantments available by anointing Rings");
    console.log(s);
    // console.log(s.json());

    var oil2 = await fetch("List of ring anointments");
};
