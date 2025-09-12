const puppeteer = require('puppeteer');

async fucntion searchBing(query) {
  const bingBrowser = await puppeteer.launch({headless: "new"});
  const bingPage = await bingBrowser.newPage();
    bingPage.setDefaultNavigationTimeout(60000);

  await bingPage.goto('https://www.bing.com/search?q=${query}');

  await bingPage.waitForSelector('#b_results');

  const bingResults = await bingPage.evaluate(( => {
    const resultElements = document.querySelectorAll('.b_algo');
    const results = [];
    for (let element of resultElements) {
      const titleElement = element.querySelector('h2');
      const linkElement = element.querySelector('a');
      const title = titleElement ? titleElement.innerText: '';
      const link = linkElement ? linkElement.href : '';

      results.push({title, link});
    }
    return results;
  }));

  await bingBrowser.close();

  return bingResults;
}

module.exports = searchBing;
